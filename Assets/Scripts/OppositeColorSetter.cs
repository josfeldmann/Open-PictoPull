using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OppositeColorSetter : MonoBehaviour
{

    public List<OppositeColorImage> positiveImage = new List<OppositeColorImage>();
    public List<OppositeColorImage> negativeImage = new List<OppositeColorImage>();
    public GameObject positiveAdd, negativeAdd;
    public List<int> posInt = new List<int>();
    public List<int> negInt = new List<int>();

    public void Set(OppositePairs pairs) {
        if (pairs == null) {
            posInt = new List<int>();
            negInt = new List<int>();
        } else {
            negInt = new List<int>(pairs.negativeIndexes);
            posInt = new List<int>(pairs.positiveIndexes);
        }
        SetColorIndexes();
    }

    public OppositePairs GetPairs() {

        if (posInt == null) posInt = new List<int>();
        if (negInt == null) negInt = new List<int>();

        if (posInt.Count == 0) {
            return null;
        } 
        if (negInt.Count == 0) {
            return null;
        }
        OppositePairs pairs = new OppositePairs();
        pairs.positiveIndexes = new List<int>(posInt);
        pairs.negativeIndexes = new List<int>(negInt);
        return pairs;
    }

    public void SetColorIndexes() {

        


        for (int i = 0; i < positiveImage.Count; i++) {
            if (i < posInt.Count) {
                positiveImage[i].gameObject.SetActive(true);
                positiveImage[i].Set(posInt[i], GameMasterManager.instance.creator.GetColorIndex(posInt[i]));

            } else {
                positiveImage[i].gameObject.SetActive(false);
            }
            positiveImage[i].setter = this;
        }
        for (int i = 0; i < negativeImage.Count; i++) {
            if (i < negInt.Count) {
               negativeImage[i].gameObject.SetActive(true);
               negativeImage[i].Set(negInt[i], GameMasterManager.instance.creator.GetColorIndex(negInt[i]));
            } else {
               negativeImage[i].gameObject.SetActive(false);
            }
            negativeImage[i].setter = this;
        }
        negativeAdd.gameObject.SetActive(negInt.Count < negativeImage.Count);
        positiveAdd.gameObject.SetActive(posInt.Count < positiveImage.Count);


    }

    public void AddNegative() {
        if (negInt.Contains(GameMasterManager.instance.creator.colorIndex)) return;
        if (posInt.Contains(GameMasterManager.instance.creator.colorIndex)) return;
        negInt.Add(GameMasterManager.instance.creator.colorIndex);
        SetColorIndexes();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void AddPositive() {
        if (negInt.Contains(GameMasterManager.instance.creator.colorIndex)) return;
        if (posInt.Contains(GameMasterManager.instance.creator.colorIndex)) return;
        posInt.Add(GameMasterManager.instance.creator.colorIndex);
        SetColorIndexes();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void RemoveNegative(int index) {
        negInt.Remove(index);
        SetColorIndexes();
    }

    public void RemovePositive(int index) {
        posInt.Remove(index);
        SetColorIndexes();
    }


}
