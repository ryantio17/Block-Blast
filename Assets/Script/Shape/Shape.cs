using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler
    
{
    [Header("Audio SFX")]
    public AudioClip holdSfx;   
    public AudioClip placedSfx;
    public AudioClip notplaceSfx;      // File suara saat balok dipegang
    private AudioSource audioSource_;
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f);
    
    [HideInInspector]
    public ShapeData CurrentShapeData;

    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;

    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _startPosition = _transform.localPosition;
        _shapeActive = true;

        audioSource_ = GetComponent<AudioSource>(); //Audio
        if (audioSource_ == null)
        {
            audioSource_ = gameObject.AddComponent<AudioSource>();
        }
        audioSource_.playOnAwake = false;
    }

    private void OnEnable()
    {
        TheGameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        TheGameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        TheGameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        TheGameEvents.SetShapeInactive-= SetShapeInactive;
    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if(square.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    public void DeactivateShape()
    {
        if(_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }
        _shapeActive = false;
    }

    private void SetShapeInactive()
    {
        if(IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void ActivateShape()
    {
        if(!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }

        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
       if (_isDraggingThisShape && !IsOnStartPosition() && placedSfx != null && audioSource_ != null)
        {
            audioSource_.PlayOneShot(placedSfx);
        }

        transform.localPosition = _startPosition;
        createShape(shapeData);
    }

    public void createShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);
        
        while(_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        //set positions to form final shape
        for(var row = 0; row < shapeData.rows; row++)
        {
            for(var column = 0; column <shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                            GetYPositionForShapeSquare(shapeData, row, moveDistance));

                            currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if(shapeData.rows > 1)
        {
            if(shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows -1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if(row < middleSquareIndex) // move it on minus
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if(row > middleSquareIndex) //move it on plus
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 2;
                var multiplier = shapeData.rows / 2;

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                     if(row == middleSquareIndex2)
                        shiftOnY = (moveDistance.y /2) * -1;
                     
                     if(row == middleSquareIndex1)
                        shiftOnY = (moveDistance.y / 2);
                }

                if(row < middleSquareIndex1 && row < middleSquareIndex2) // move it on minus
                {
                    shiftOnY = moveDistance.y *1;
                    shiftOnY *= multiplier;
                }
                else if(row > middleSquareIndex1 && row > middleSquareIndex2) // move it on plus
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
        }

        return shiftOnY;
    }

    
    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if (shapeData.columns > 1) //Vertical position calculation 
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex= (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if(column < middleSquareIndex) //move it on the negative
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex) // move it on plus
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }        
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns / 2;

                if(column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if(column == middleSquareIndex2)
                        shiftOnX = moveDistance.x / 2;
                    if(column == middleSquareIndex1)
                    shiftOnX = (moveDistance.x / 2) * -1;
                }

                if(column < middleSquareIndex1 && column <middleSquareIndex2) //move it on negative
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2) // move it on plus
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
        }

        return shiftOnX;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                    number++;
            }
        }

        return number;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
    private bool _wasPlacedThisTurn = false;
    private bool _isDraggingThisShape = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDraggingThisShape = true;

        // Putar suara sekali saat balok mulai disentuh/dipegang
        if (holdSfx != null && audioSource_ != null)
        {
            audioSource_.PlayOneShot(holdSfx);
        }
        
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        _wasPlacedThisTurn = false;

        TheGameEvents.CheckIfShapeCanBePlaced();

       // Hanya bunyi jika balok INI yang barusan di-drag dan berhasil ditaruh
        if (_isDraggingThisShape && !_wasPlacedThisTurn && !IsOnStartPosition())
        {
            if (placedSfx != null && audioSource_ != null)
            {
                audioSource_.PlayOneShot(placedSfx);
            }
        }

        _isDraggingThisShape = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    
    }

    private void MoveShapeToStartPosition()
    {
        // Hanya balok yang sedang di-drag yang boleh membunyikan notplaceSfx
        if (_isDraggingThisShape && !IsOnStartPosition())
        {
            _wasPlacedThisTurn = true;

            if (notplaceSfx != null && audioSource_ != null)
            {
                audioSource_.PlayOneShot(notplaceSfx);
            }
        }
        _transform.transform.localPosition = _startPosition;
    }
}