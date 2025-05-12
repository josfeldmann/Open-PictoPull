using UnityEngine;

using System.IO;
using System.Collections;
#if !DISABLESTEAMWORKS
//using LapinerTools.Steam;
//using LapinerTools.Steam.UI;
//using LapinerTools.Steam.Data;
//using LapinerTools.uMyGUI;

public class TestSteamWorkshopUpload : MonoBehaviour {
	private void Start() {
		// enable debug log
	//	SteamWorkshopMain.Instance.IsDebugLogEnabled = true;

		// everything inside this folder will be uploaded with your item

		string datapath = Application.persistentDataPath;
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			datapath = Application.dataPath;
        }

		string dummyItemContentFolder = Path.Combine(datapath, "DummyItemContentFolder" + System.DateTime.Now.Ticks); // use DateTime.Now.Ticks to create a unique folder for each upload
		if (!Directory.Exists(dummyItemContentFolder)) { Directory.CreateDirectory(dummyItemContentFolder); }

		// create dummy content to upload
		string dummyItemContentStr =
			"Save your item/level/mod data here.\n" +
			"It does not need to be a text file. Any file type is supported (binary, images, etc...).\n" +
			"You can save multiple files, Steam items are folders (not single files).\n";
		File.WriteAllText(Path.Combine(dummyItemContentFolder, "ItemData.txt"), dummyItemContentStr);

		


		// tell which folder you want to upload
		//WorkshopItemUpdate createNewItemUsingGivenFolder = new WorkshopItemUpdate();
		//createNewItemUsingGivenFolder.ContentPath = dummyItemContentFolder;

		// show the Steam Workshop item upload popup
		//((SteamWorkshopPopupUpload)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_upload")).UploadUI.SetItemData(createNewItemUsingGivenFolder);
	}
}
#endif