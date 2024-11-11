using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.Dynamics;

public partial class Yuukirin_BatchAddCollision : EditorWindow {

    [MenuItem("YUUKIRIN/コリジョン一括追加")]
    private static void ShowWindow()
    {
        var window = GetWindow<Yuukirin_BatchAddCollision>("UIElements");
        window.titleContent = new GUIContent("ゆうきりん/コリジョン一括追加");
        window.Show();
    }
    private ObjectField YUUKIRINBatchAddCollisionTargetField;
    private ObjectField YUUKIRINBatchAddCollisionTargetCollisionField;
    private Button button;
    private HelpBox YUUKIRINBatchAddCollisionTargetErrorElement;
    
    private HelpBox YUUKIRINBatchAddCollisionTargetCollisionErrorElement;
    private HelpBox YUUKIRINBatchAddCollisionTargetCollisionResultElement;
    
    private void CreateGUI() {
        VisualElement root = rootVisualElement;
        Label Label1 = new(){
            text="コリジョンを適用したいルートボーン"
        };
        YUUKIRINBatchAddCollisionTargetField = new(){
            objectType = typeof(GameObject),
        };
        YUUKIRINBatchAddCollisionTargetErrorElement = new(){
            messageType = HelpBoxMessageType.Error,
            text = "コリジョンを適用したいルートボーンが指定されていません。",
            visible = false
        };
        Label Label2 = new(){
            text="コリジョン"
        };
        YUUKIRINBatchAddCollisionTargetCollisionField = new(){
            objectType = typeof(VRCPhysBoneCollider)
        };
        YUUKIRINBatchAddCollisionTargetCollisionErrorElement = new(){
            messageType = HelpBoxMessageType.Error,
            text = "コリジョンが指定されていません。",
            visible = false
        };
        button = new(){
            text = "適用"
        };
        YUUKIRINBatchAddCollisionTargetCollisionResultElement = new() {
            messageType = HelpBoxMessageType.Info,
            text = "コリジョンを追加しました。",
            visible = false
        };
        YUUKIRINBatchAddCollisionTargetField.RegisterValueChangedCallback(
            _ => ValidateRequired<Object>(
                YUUKIRINBatchAddCollisionTargetField,
                YUUKIRINBatchAddCollisionTargetErrorElement
            ));
        YUUKIRINBatchAddCollisionTargetCollisionField.RegisterValueChangedCallback(
            _ => ValidateRequired<Object>(
                YUUKIRINBatchAddCollisionTargetCollisionField,
                YUUKIRINBatchAddCollisionTargetCollisionErrorElement
            ));

        root.Add(Label1);
        root.Add(YUUKIRINBatchAddCollisionTargetField);
        root.Add(YUUKIRINBatchAddCollisionTargetErrorElement);
        root.Add(Label2);
        root.Add(YUUKIRINBatchAddCollisionTargetCollisionField);
        root.Add(YUUKIRINBatchAddCollisionTargetCollisionErrorElement);
        root.Add(button);
        root.Add(YUUKIRINBatchAddCollisionTargetCollisionResultElement);
        button.clicked += () => {
            BatchAddCollision();
        };
    }

    private bool ValidateRequired<T>(BaseField<T> field, HelpBox helpBox) {
        T value = (T)field.value;
        helpBox.visible = value == null;
        SetResultVisible(false);
        return SetButtonEnable();
    }

    private bool ValidateAll () {
        ValidateRequired<Object>(YUUKIRINBatchAddCollisionTargetField, YUUKIRINBatchAddCollisionTargetErrorElement);
        ValidateRequired<Object>(YUUKIRINBatchAddCollisionTargetCollisionField, YUUKIRINBatchAddCollisionTargetCollisionErrorElement);
        SetResultVisible(false);
        return SetButtonEnable();
    }

    private bool YUUKIRINBatchAddCollisionTargetFieldFilled () {
        Object value = (Object)YUUKIRINBatchAddCollisionTargetField.value;
        if (!value) {
            return false;
        } else {
            return true;
        }
    }

    private bool YUUKIRINBatchAddCollisionTargetCollisionFieldFilled () {
        Object value = (Object)YUUKIRINBatchAddCollisionTargetCollisionField.value;
        if (!value) {
            return false;
        } else {
            return true;
        }
    }

    private bool SetButtonEnable () {
        button.SetEnabled(YUUKIRINBatchAddCollisionTargetFieldFilled() && YUUKIRINBatchAddCollisionTargetCollisionFieldFilled());
        return YUUKIRINBatchAddCollisionTargetFieldFilled() && YUUKIRINBatchAddCollisionTargetCollisionFieldFilled();
    }
    private void SetResultVisible (bool val) {
        YUUKIRINBatchAddCollisionTargetCollisionResultElement.visible = val;
    }

    private void BatchAddCollision() {
        if (!ValidateAll()) {
            return;
        }
        GameObject target = (GameObject)YUUKIRINBatchAddCollisionTargetField.value;
        VRCPhysBoneColliderBase collider = (VRCPhysBoneCollider)YUUKIRINBatchAddCollisionTargetCollisionField.value;
        if (!target || !collider) {
            return;
        }
        AddCollision(target, collider);
        SetResultVisible(true);
    }

    private void AddCollision(GameObject target, VRCPhysBoneColliderBase collider) {
        int childCount = target.transform.childCount;
        if (childCount > 0) {
            for (var i = 0; i < childCount; ++i)
            {
                var child = target.transform.GetChild(i).gameObject;
                AddCollision(child, collider);
            }
        }
        if (!target.GetComponent<VRCPhysBone>()) {
            return;
        }
        VRCPhysBone vrcPhysBone = target.GetComponent<VRCPhysBone>();
        vrcPhysBone.allowCollision = VRCPhysBoneBase.AdvancedBool.True;
        bool isSameCol(VRCPhysBoneColliderBase col) {
            return col == collider;
        }

        if (!vrcPhysBone.colliders.Exists(isSameCol)) {
            vrcPhysBone.colliders.Add(collider);
        }
    }

}