//Adapted from Unity Game Development Cookbook, published by O'Reilly Media in March 2019

using UnityEngine;

#if UNITY_EDITOR //Only use UnityEditor when this class is being used in the editor
using UnityEditor;
#endif
public class Checkpoint : MonoBehaviour
{
    [SerializeField] public bool isLapStart;

    [SerializeField] public Checkpoint next; //The next checkpoint in the circuit

    internal int index = 0; //Index is only accessible within this file

    private void OnDrawGizmos()
    {
        if (isLapStart)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawSphere(transform.position, 0.5f);

        if (next != null) //Draw line to next checkpoint
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Checkpoint))]

public class CheckpointEditor : Editor //Allows us to add new checkpoints in the editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Get a reference to the checkpoint component we're editing
        var checkpoint = this.target as Checkpoint;

        if (GUILayout.Button("Insert Checkpoint"))
        {
            //Make a new object called "Checkpoint", and add Checkpoint script to it
            var newCheckpoint = new GameObject("Checkpoint").AddComponent<Checkpoint>();
            newCheckpoint.next = checkpoint.next;
            checkpoint.next = newCheckpoint;

            newCheckpoint.transform.SetParent(checkpoint.transform.parent, true);

            var nextSiblingIndex = checkpoint.transform.GetSiblingIndex() + 1;

            newCheckpoint.transform.SetSiblingIndex(nextSiblingIndex);

            newCheckpoint.transform.position = checkpoint.transform.position + new Vector3(1, 0, 0);

            Selection.activeGameObject = newCheckpoint.gameObject;

            //Disable remove button if there isn't a next checkpoint, or if the next checkpoint is the lap start

            var disableRemoveButton = (checkpoint.next == null || checkpoint.next.isLapStart);

            //Display a button that removes the next checkpoint

            using (new EditorGUI.DisabledGroupScope(disableRemoveButton))
            {
                if (GUILayout.Button("Remove Next Checkpoint"))
                {
                    //Get the node this next checkpoint was linking to...
                    var next = checkpoint.next.next;
                    //remove the next checkpoint...
                    DestroyImmediate(checkpoint.next.gameObject);
                    //and aim ourselves at the checkpoint the destroyed one was looking at
                    checkpoint.next = next;
                }
            }



        }
    }
}
#endif
