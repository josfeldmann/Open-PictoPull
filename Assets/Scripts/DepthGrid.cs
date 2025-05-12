using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DepthGrid : MonoBehaviour
{
    public RectTransform gridRect;
    public RectTransform toWatchRect;
    public DepthGridButton textPrefab;
    public List<DepthGridButton> textGrid;
    public Transform groupingObject;
    public BlockLevelCreator blockLevelCreator;
    public GridLayoutGroup gridLayout;
    public GameObject showDepthButton;
    public Vector2 size = new Vector2(750, 750);

    public DepthGridButton[,] bakcupGrid;

    bool setUpYet = false;


    public void EnsureDepthGridIsClear() {
        foreach (DepthGridButton button in textGrid) {
            button.SetDepthValue(0);
        }
    }

    public void UpdateGrid() {
        if (setUpYet) {
            textGrid = new List<DepthGridButton>();
        }

        int toAdd = (blockLevelCreator.width * blockLevelCreator.height) - textGrid.Count;

        for (int i = 0; i < toAdd; i++) {
            DepthGridButton button = Instantiate(textPrefab,groupingObject);
            textGrid.Add(button);
        }

        foreach (DepthGridButton b in textGrid) {
            b.gameObject.SetActive(false);
        }

        int[,] grid = new int[blockLevelCreator.width, blockLevelCreator.height];
        bool copy = false;
        if (bakcupGrid != null) {
            copy = true;
            for (int x = 0; x < blockLevelCreator.width; x++) {
                for (int y = 0; y < blockLevelCreator.height; y++) {

                    Vector3Int pos = BlockLevelCreator.newStart + new Vector3Int(x, y);

                    if (pos.x < bakcupGrid.GetLength(0) && pos.y < bakcupGrid.GetLength(1)) {
                        grid[x, y] = bakcupGrid[pos.x, pos.y].depthValue;
                    }

                }
            }
        }


        bakcupGrid = new DepthGridButton[blockLevelCreator.width, blockLevelCreator.height];

        

        int idx = 0;
        for (int y = 0; y < blockLevelCreator.height; y++) {
            for (int x = 0; x < blockLevelCreator.width; x++) {
                DepthGridButton g = textGrid[idx];
                g.gameObject.SetActive(true);
            //    Debug.Log("Reset");
                if (copy) {
                    g.SetDepthValue(grid[x, y]);
                } else {
                    g.SetDepthValue(0);
                }
                g.SetCoord(new Vector2Int(x, y));
                idx++;
                bakcupGrid[x, y] = g;
            }
        }

        
        int val = blockLevelCreator.width;
        if (blockLevelCreator.height > blockLevelCreator.width) {
            val = blockLevelCreator.height;
        } else {
            
        }




        gridLayout.cellSize = size / val;
        gridRect.sizeDelta = toWatchRect.sizeDelta;





    }


    public void SetGridValue(int[,] depthGrid) {
        for (int x = 0; x < blockLevelCreator.width; x++) {
            for (int y = 0; y < blockLevelCreator.height; y++) {
                bakcupGrid[x, y].SetDepthValue(depthGrid[x,y]);
            }
        }
    }

    public int CoordToIndex(int x, int y) {
        return (x * blockLevelCreator.width) + y;
    }

    public int GetValue(int x, int y) {
        return bakcupGrid[x,y].depthValue;
    }

    internal void Hide() {
        //showDepthButton.SetActive(true);
        groupingObject.gameObject.SetActive(false);
    }

    internal void Show() {
       // showDepthButton.SetActive(false);
        groupingObject.gameObject.SetActive(true);
    }

    public void ToggleDepthGrid() {
        EventSystem.current.SetSelectedGameObject(null);
        if (groupingObject.gameObject.activeInHierarchy) {
            blockLevelCreator.HideDepthGrid();
        } else {
            blockLevelCreator.ShowDepthGrid();
        }
    }

}
