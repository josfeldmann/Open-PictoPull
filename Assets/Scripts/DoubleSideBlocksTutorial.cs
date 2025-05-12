using UnityEngine;

public class DoubleSideBlocksTutorial : MonoBehaviour {

    public CutSceneManager manager;

    public void OnEnable() {

        if (GameMasterManager.instance == null) return;
        if (GameMasterManager.instance.generator.blocks.Count < 7) return;
        block1 = GameMasterManager.instance.generator.GetBlockNumber(0);
        block2 = GameMasterManager.instance.generator.GetBlockNumber(1);
        block3 = GameMasterManager.instance.generator.GetBlockNumber(6);
        block4 = GameMasterManager.instance.generator.GetBlockNumber(2);
        block5 = GameMasterManager.instance.generator.GetBlockNumber(3);
        block6 = GameMasterManager.instance.generator.GetBlockNumber(4);
        block7 = GameMasterManager.instance.generator.GetBlockNumber(5);





    }

    public void Update() {
        SetPosition();
    }

   

    


    public Block block1, block2, block3, block4, block5, block6, block7;


    public void SetPosition() {
        if (manager.player.isPulling()) {
            manager.generator.arrowIndicator.Hide();
            return;
        }
       
        manager.generator.arrowIndicator.Show();
        if (block1.currentPullLevel < 3) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(block1)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 0, -0.5f), block1);
        } else if (block2.currentPullLevel < 2) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(block2)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(2, 1, -0.5f), block2);
        } else if (block3.currentPullLevel < 1) {
           
            if (!manager.generator.arrowIndicator.IsAttachedTo(block3)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 2, -0.5f), block3);
        } else if (block3.currentPullLevel == 1) {
           if (manager.generator.arrowIndicator.transform.localPosition != new Vector3(0, 2, 0.5f)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.RIGHT, new Vector3(0, 2, 0.5f), block3);
        } else if (block3.currentPullLevel == 2) {
            if (manager.generator.arrowIndicator.transform.localPosition != new Vector3(0, 2, 1.5f)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.RIGHT, new Vector3(0, 2, 1.5f), block3);
        } else if (block4.currentPullLevel < 2) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(block4)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(2, 3, -0.5f), block4);
        } else if (block5.currentPullLevel < 1) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(block5)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 4, -0.5f), block5);
        } else if (block5.currentPullLevel == 1) {
            if (manager.generator.arrowIndicator.transform.localPosition != new Vector3(3, 4, 0.5f)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.LEFT, new Vector3(3, 4, 0.5f), block5);
        } else if (block5.currentPullLevel == 2) {
            if (manager.generator.arrowIndicator.transform.localPosition != new Vector3(3, 4, 1.5f)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.LEFT, new Vector3(3, 4, 1.5f), block5);
        } else if (block6.currentPullLevel < 2) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(block6)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(2, 5, -0.5f), block6);
        } else if (block7.currentPullLevel < 1) {
            if (!manager.generator.arrowIndicator.IsAttachedTo(block7)) manager.generator.arrowIndicator.SetPosition(BlockDirection.DOWN, BlockDirection.UP, new Vector3(1, 6, -0.5f), block7);
        } else {
           // Debug.Log(block3.currentPullLevel + " " + Time.time);
           manager.generator.arrowIndicator.Hide();
        }

    }
}


