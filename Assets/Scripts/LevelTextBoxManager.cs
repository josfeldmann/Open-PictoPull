using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTextBoxManager : MonoBehaviour
{
    public int textNumber = 0;

    public List<LevelTextBox> boxes = new List<LevelTextBox>();
    public Button addButton;

    public void HideAll() {
        foreach (LevelTextBox box in boxes) {
            box.gameObject.SetActive(false);
        }
        textNumber = 0;
        ButtonCheck();
    }

    public void AddBox() {
        if (textNumber > boxes.Count) {
            return;
        }

        textNumber++;
        boxes[textNumber - 1].inputField.SetTextWithoutNotify("");
        boxes[textNumber - 1].gameObject.SetActive(true);
        ButtonCheck();
    }

    public void RemoveBox(LevelTextBox box) {
        if (!box.gameObject.activeInHierarchy) {
            return;
        }
        int index = boxes.IndexOf(box);
        if (index == textNumber - 1) {
            textNumber--;
            boxes[index].gameObject.SetActive(false);
            return;
        } else {
            for (int i = 0; i < boxes.Count; i++) {
                if (i >= index && i < boxes.Count - 1) {
                    boxes[i].inputField.SetTextWithoutNotify(boxes[i + 1].inputField.text);
                }
            }
            boxes[textNumber - 1].gameObject.SetActive(false);
            textNumber--;
        }
        ButtonCheck();

    }


    public void SetBasedOnStringList(List<string> s) {
        HideAll();

        if (s == null || s.Count == 0) {
            return;
        }
        
        foreach (string ss in s) {
            AddBox();
        }

        for (int i = 0; i < s.Count; i++) {
            boxes[i].gameObject.SetActive(true);
            boxes[i].inputField.SetTextWithoutNotify(s[i]);
        }
        
    }

    public void ButtonCheck() {
        if (textNumber == boxes.Count) {
            addButton.interactable = false;
        } else {
            addButton.interactable = true;
        }
    }

    internal List<string> GetStringList() {
        List<string> messages = new List<string>();

        for (int i = 0; i < textNumber; i++) {
            messages.Add(boxes[i].inputField.text);
        }

        return messages;
    }
}
