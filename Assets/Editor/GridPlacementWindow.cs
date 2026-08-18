using UnityEngine;
using UnityEditor;

public class GridPlacementWindow : EditorWindow
{
    private GameObject prefabToPlace;
    private float gridSize = 1.0f;
    private bool isPlacementModeActive = false;

    [MenuItem("Tools/Escorrega Huguinho/Grid Placement Tool")]
    public static void ShowWindow()
    {
        GetWindow<GridPlacementWindow>("Grid Placement");
    }

    private void OnGUI()
    {
        GUILayout.Label("Configurações do Grid", EditorStyles.boldLabel);

        // Campo para selecionar o Prefab
        prefabToPlace = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToPlace, typeof(GameObject), false);

        // Tamanho do Grid (limitado para não ser zero ou negativo)
        gridSize = Mathf.Max(0.1f, EditorGUILayout.FloatField("Tamanho do Grid", gridSize));

        // Ativar/Desativar modo de posicionamento
        isPlacementModeActive = EditorGUILayout.Toggle("Ativar Posicionamento", isPlacementModeActive);

        if (isPlacementModeActive)
        {
            EditorGUILayout.HelpBox("Como usar:\n1. Mantenha SHIFT pressionado no Scene View.\n2. Clique com o botão esquerdo do mouse para posicionar no Grid.", MessageType.Info);
        }
    }

    private void OnEnable()
    {
        // Inscreve o método para rodar durante a renderização do Scene View
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Remove a inscrição para evitar vazamento de memória
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacementModeActive || prefabToPlace == null)
            return;

        // Captura o ID de controle para gerenciar o foco do clique
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Event currentEvent = Event.current;

        // Cria um plano virtual no ponto Y = 0 (ajustável se necessário)
        Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            // Cálculo matemático de Snapping (Grid)
            float snappedX = Mathf.Round(hitPoint.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(hitPoint.z / gridSize) * gridSize;
            Vector3 snappedPosition = new Vector3(snappedX, 0, snappedZ);

            // Desenha o cubo de pré-visualização verde no Scene View
            Handles.color = Color.green;
            Handles.DrawWireCube(snappedPosition, Vector3.one * gridSize);

            // Força a atualização visual da cena para suavizar o desenho
            sceneView.Repaint();

            // Intercepta o clique se Shift estiver pressionado
            if (currentEvent.shift && currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                // Consome o evento para evitar que o Unity mude a seleção ativa
                GUIUtility.hotControl = controlID;

                // Instancia como um Prefab real (mantendo o link azul no Unity 6)
                GameObject instantiatedObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
                instantiatedObj.transform.position = snappedPosition;

                // Registra no sistema de Undo para que Ctrl+Z funcione perfeitamente
                Undo.RegisterCreatedObjectUndo(instantiatedObj, "Instanciar Prefab no Grid");

                // Consome o evento de clique do mouse
                currentEvent.Use();
            }

            // Libera o controle ao soltar o clique
            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                GUIUtility.hotControl = 0;
            }
        }
    }
}
