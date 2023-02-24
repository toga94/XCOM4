using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private LayerMask gridObjectLayer;
    [SerializeField] private float gridSize = 0.1f;
    private Transform _draggableObject;
    private Character character;
    private Vector3 _startDraggablePosition;
    private float _dragDistance;
    private Vector3 _offset;
    private Camera _mainCamera;
    private LevelGrid levelGrid;


    public Unit[] freeUnits;
    private void Start()
    {
        levelGrid = LevelGrid.Instance;
        _mainCamera = Camera.main;

        //AddtoGrid();
    }

    private void AddtoGrid()
    {
        int unitIndex = 0;

        for (int y = levelGrid.GetHeight() - 1; y >= 0; y--)
        {
            for (int x = levelGrid.GetWidth() - 1; x >= 0; x--)
            {
                GridPosition gridPosition = new GridPosition(x, y);

                levelGrid.AddUnitAtGridPosition(gridPosition, freeUnits[unitIndex]);
                freeUnits[unitIndex].Move(levelGrid.GetWorldPosition(gridPosition));
                //freeUnits[unitIndex].addedtoGrid = true;
                unitIndex++;

                if (unitIndex >= freeUnits.Length)
                {
                    unitIndex = 0;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.touchCount != 1)
        {
            StopDragging();
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            StartDragging(touch);
        }

        else if (_draggableObject && touch.phase == TouchPhase.Moved)
        {
            character.GetAnimator.SetBool("fall", true);
            MoveDraggableObject(touch);
        }

        else if (_draggableObject && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            character.GetAnimator.SetBool("fall", false);
            StopDragging();
        }
    }

    private void StartDragging(Touch touch)
    {
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && IsLayer(hit.collider.gameObject.layer, draggableLayer))
        {
            _draggableObject = hit.transform;

            character = new Character(
                _draggableObject, 
                hit.transform.GetComponent<Collider>(), 
                hit.transform.GetComponent<Animator>(),
                hit.transform.GetComponent<Unit>()
                );


            _startDraggablePosition = _draggableObject.position;
            _dragDistance = character.GetTransform.position.z - _mainCamera.transform.position.z;
            Vector3 position = new Vector3(touch.position.x, touch.position.y, _dragDistance);
            position = _mainCamera.WorldToScreenPoint(character.GetTransform.position);
            _offset = character.GetTransform.position - _mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, position.z));
        }
    }
    private Transform lastfloor;
    private void MoveDraggableObject(Touch touch)
    {
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridObjectLayer))
        {
            float x = Mathf.Floor(hit.point.x / gridSize) * gridSize;
            float y = Mathf.Floor(hit.point.y / gridSize) * gridSize + 2;
            float z = Mathf.Floor(hit.point.z / gridSize) * gridSize;

            Vector3 cursorPosition = new Vector3(x, y, z) + _offset;
            character.GetTransform.position = cursorPosition;
            lastfloor = hit.transform;
        }
        character.GetCollider.enabled = false;
    }

    private void StopDragging()
    {
        if (_draggableObject)
        {
            if(!lastfloor) 
                character.GetTransform.position = _startDraggablePosition;
            else
            {
                GridPosition lastgridPosition = levelGrid.GetGridPosition(_startDraggablePosition);
                GridPosition gridPosition = levelGrid.GetGridPosition(lastfloor.position);

                if(levelGrid.IsValidGridPosition(levelGrid.GetGridPosition(lastfloor.position)))
                if (!levelGrid.HasAnyUnitOnGridPosition(gridPosition))
                {
                        Unit unit = character.GetUnit;
                        levelGrid.UnitMovedGridPosition(unit, lastgridPosition, gridPosition);
                        
                        unit.Move(levelGrid.GetWorldPosition(gridPosition));
                        levelGrid.RemoveAnyUnitAtGridPosition(lastgridPosition);
                        Debug.Log(levelGrid.HasAnyUnitOnGridPosition(lastgridPosition));
                    }
                else
                {

                        levelGrid.UnitSwappedGridPosition(character.GetUnit, levelGrid.GetUnitAtGridPosition(gridPosition), gridPosition, lastgridPosition);

                }
            }

            character.GetCollider.enabled = true;
            character = null;
            _draggableObject = null;
        }
    }

    private bool IsLayer(int gameObjectLayer, LayerMask layer)
    {
        return ((1 << gameObjectLayer) & layer) != 0;
    }
}