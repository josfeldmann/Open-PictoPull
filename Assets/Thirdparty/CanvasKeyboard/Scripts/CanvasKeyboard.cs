using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace CanvasKeyboard {


    public class CanvasKeyboard : MonoBehaviour {

        [Header("Restrictions")]
        public bool removeSpecialCharacters = false;
        public bool removeWhiteSpace = false;
        public bool removeTabs = false;
        public bool initializeOnAwake = false;
        public bool useKeyboard = false;

        [Header("Validation")]
        public bool stringCantBeEmpty = true;
        public bool stringCantBeJustWhiteSpace = true;


        public List<char> prohibitedCharacters = new List<char>();

        public KeyboardData data;
        public string buildString = "";
        public UnityEvent<string> onStringEdit;

        public KeyboardRow keyBoardRowPrefab;
        public List<KeyboardRow> rows = new List<KeyboardRow>();
        public Transform keyRowGroupingObject;
        public VerticalLayoutGroup verticalLayoutGroup;
        public int maxBuildStringSize = 12;
        public bool isShifted = false;

        public Button doneButton;
        public static bool initYet = false;

        private void Awake() {
            if (!initYet) {
                Keyboard.current.onTextInput += PressKey;
                initYet = true;
            }
        }

        public void Setup() {
            if (initializeOnAwake) {
                Setup(false);
            }
            onStringEdit.Invoke(buildString);
            CheckDone();
        }
        

        private void Update() {
            if (useKeyboard) {
                KeyboardUpdate();
            }
        }

        public void Setup(bool calledFromEditor) {
            foreach (KeyboardRow k in rows) {
                if (calledFromEditor) {
                    DestroyImmediate(k.gameObject);
                } else {
                    Destroy(k.gameObject);
                }
            }
            rows.Clear();
            SetData();
            verticalLayoutGroup.childForceExpandHeight = false;
            SetShifted(false);
        }

        public void SetShifted(bool v) {
            isShifted = v;
            foreach (KeyboardRow r in rows) {
                foreach (KeyboardKey k in r.keys) {
                    k.SetShift(v);
                }
            }
        }

        internal void PressDoneButton() {
          
            CheckDone();
            if (canBeDone) {
                isDone = true;
                isCancelled = false;
            }
        }

        public void PressCancelButton() {
            isDone = true;
            isCancelled = true;
        }

     

        public static string RemoveSpecificCharacters(string str, List<char> toRemove) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if (!toRemove.Contains(c)) {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        public static string RemoveSpecialCharacters(string str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveWhiteSpace(string str) {
            return String.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
        }

        public void SetData() {
            int toAdd = data.keyDataRow.Count - rows.Count;
            for (int i = 0; i < toAdd; i++) {
                rows.Add(Instantiate(keyBoardRowPrefab, keyRowGroupingObject));
            }
            for (int x = 0; x < rows.Count; x++) {
                if (x <= data.keyDataRow.Count) {
                    rows[x].gameObject.SetActive(true);
                    rows[x].SetKeyboardRow(data.keyDataRow[x], this);
                    
                } else {
                    rows[x].gameObject.SetActive(false);
                }
            }
        }

        public void BackSpace() {
            if (buildString.Length == 0) return;
            buildString = buildString.Remove(buildString.Length - 1);
            SanitizeBuildString();
        }

        public void ShiftToggle() {
            //Debug.Break();
            SetShifted(!isShifted);
        }

        public void PressKey(char c) {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
                if (buildString.Length >= maxBuildStringSize) return;
                buildString += c.ToString();
                SanitizeBuildString();
            }
        }

        public void AddTab() {
            PressKey('\t');
        }

        public void SetBuildString(string s) {

            buildString = s;
            SanitizeBuildString();
        }

        public void SanitizeBuildString() {
            if (buildString.Length > maxBuildStringSize) {
                buildString = buildString.Substring(0, maxBuildStringSize);
            }
            if (removeSpecialCharacters) {
                buildString = RemoveSpecialCharacters(buildString);
            }
            if (removeWhiteSpace) {
                buildString = RemoveWhiteSpace(buildString);
            }
            if (prohibitedCharacters.Count > 0 ) {
                buildString = RemoveSpecificCharacters(buildString, prohibitedCharacters);
            }
            if (removeTabs) {
                buildString = buildString.Replace("\t", String.Empty);
            }
            CheckDone();
            onStringEdit.Invoke(buildString);
        }

        public float minKeyCooldown = 0.1f;
        private float lastTime = 0;

        public void KeyCoolDown() {
            lastTime = Time.time + minKeyCooldown; 
        }

        public void KeyboardUpdate() {
            if (lastTime > Time.time) return;
            if (EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
                return;
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame) {
                ShiftToggle();
            }
            else if (Keyboard.current.tabKey.wasPressedThisFrame) {
                AddTab();
                KeyCoolDown();
            } else if (Keyboard.current.backspaceKey.wasPressedThisFrame ||
                       Keyboard.current.deleteKey.wasPressedThisFrame) { 
                BackSpace();
                KeyCoolDown();
            }
        }

        public void AddString(string s) {
            buildString += s;
            SanitizeBuildString();
        }
        public bool isDone = false;
        public bool isCancelled = false;
        public bool canBeDone;
        public int minStringBuildSize;

        public void CheckDone() {
            if (CheckIfKeyboardCanSubmit()) {
                canBeDone = true;
            } else {
                canBeDone = false;
            }
            if (doneButton) doneButton.interactable = canBeDone;
        }

        public void DoneButton() {
            if (canBeDone) {
                isDone = true;
            } else {
                isDone = false;
            }
        }

        public bool CheckIfKeyboardCanSubmit() {
            if (stringCantBeEmpty && buildString.Length == 0) return false;
            if (stringCantBeJustWhiteSpace && IsStringOnlyWhiteSpace(buildString)) return false;
            if (buildString.Length < minStringBuildSize || buildString.Length > maxBuildStringSize) return false;
            return true;
        }


        public bool IsStringOnlyWhiteSpace(string s) {
            return s == null || s.Length == 0 || s.All(char.IsWhiteSpace);
        }
    }


    [System.Serializable]
    public class KeyDataRow {
        public List<KeyData> keyData = new List<KeyData>();
    }

    [System.Serializable]
    public class KeyData {
        public char normalChar;
        public char shiftChar;
        public KeyType keyType;
    }


    public enum KeyType { LETTERCHAR = 0, SHIFTCHAR = 1, SINGULARCHAR = 2, SPACE = 3, SHIFT = 4, TAB = 5, BACKSPACE = 6, DONE = 7, CANCEL = 8 }

    
}


