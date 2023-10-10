using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    #region Variables

        #region Singleton

            public static InteractionSystem instance = null;

        #endregion

        #region Settings

            [Header("Settings")]
            [SerializeField] private float shortInteractionRange;
            [SerializeField] private float longInteractionRange;

        #endregion

        #region DEBUG

            [HideInInspector] public Vector3 flatSurfacePosition;
            [HideInInspector] public float flatSurfaceRotationX;
            [HideInInspector] public float flatSurfaceRotationY;
            [HideInInspector] public float flatSurfaceRotationZ;
            [HideInInspector] public bool resumeInteraction = false;
            [HideInInspector] public bool isHoldingAnObject = false;
            [HideInInspector] public bool lockInteractionReticle = false;
            private bool objectIsFurtherAway = false;

        #endregion

        #region Components

            private InteractableObject interactableObject;
            private PickableObject pickableObject;
            [HideInInspector] public PickableObject scriptOfObjectPlayerIsHolding;
            private FlatSurface flatSurface;
            private ActionSurface actionSurface;
            private CustomSurface customSurface;
            private ViewableObject viewableObject;
            private Ladder ladder;

            private Camera mainCamera;
            private RaycastHit hit;

        #endregion

    #endregion

    #region Built-in Method

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if(resumeInteraction)
            {
                Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, longInteractionRange);
                objectIsFurtherAway = false;

                #region Getting Object Information

                    if(hit.transform)
                    {
                        if(shortInteractionRange < hit.distance)
                        {
                            objectIsFurtherAway = true;
                        }

                        interactableObject = hit.transform.GetComponent<InteractableObject>();
                        pickableObject = hit.transform.GetComponent<PickableObject>();
                        flatSurface = hit.transform.GetComponent<FlatSurface>();
                        actionSurface = hit.transform.GetComponent<ActionSurface>();
                        customSurface = hit.transform.GetComponent<CustomSurface>();
                        viewableObject = hit.transform.GetComponent<ViewableObject>();
                        ladder = hit.transform.GetComponent<Ladder>();

                        if(flatSurface)
                        {
                            flatSurfacePosition = hit.transform.position;

                            flatSurfaceRotationX = hit.transform.rotation.x;
                            flatSurfaceRotationY = hit.transform.rotation.y;
                            flatSurfaceRotationZ = hit.transform.rotation.z;
                        }
                    }
                    else
                    {
                        interactableObject = null;
                        pickableObject = null;
                        flatSurface = null;
                        actionSurface = null;
                        customSurface = null;
                        viewableObject = null;
                        ladder = null;
                    }

                #endregion

                #region Interactable Object Check

                    if(!objectIsFurtherAway)
                    {
                        if(interactableObject)
                        {
                            if(!interactableObject.isMoving && !interactableObject.controlledObjectIsMoving)
                            {
                                if(!interactableObject.hasTrigger)
                                {
                                    InteractableObjectCheck();
                                }
                                else
                                {
                                    if(interactableObject.enteredTrigger)
                                    {
                                        InteractableObjectCheck();
                                    }
                                    else
                                    {
                                        ObjectIsNotInteractable();
                                    }
                                }
                            }
                            else
                            {
                                ObjectIsNotInteractable();
                            }
                        }
                        else if(pickableObject)
                        {
                            if(!isHoldingAnObject)
                            {
                                ObjectIsInteractable();
                            }
                            else if(scriptOfObjectPlayerIsHolding.shoppingBasket)
                            {
                                if(scriptOfObjectPlayerIsHolding.basket.ItemCheck(pickableObject, false))
                                {
                                   ObjectIsInteractable();
                                }
                                else
                                {
                                   ObjectIsNotInteractable();
                                }
                            }
                            else
                            {
                                ObjectIsNotInteractable();
                            }
                        }
                        else if(flatSurface)
                        {
                            if(isHoldingAnObject)
                            {
                                if(flatSurface.hasAnObject)
                                {
                                    ObjectIsNotInteractable();
                                }
                                else
                                {
                                    ObjectIsInteractable();
                                }
                            }
                            else
                            {
                                ObjectIsNotInteractable();
                            }
                        }
                        else if(actionSurface)
                        {
                            if(isHoldingAnObject)
                            {
                                if(!actionSurface.hasAnObject)
                                {
                                    if(actionSurface.RequiredObject(scriptOfObjectPlayerIsHolding))
                                    {
                                        ObjectIsInteractable();
                                    }
                                    else
                                    {
                                        ObjectIsNotInteractable();
                                    }
                                }
                                else
                                {
                                    ObjectIsNotInteractable();
                                }
                            }
                            else
                            {
                                if(actionSurface.hasAnObject)
                                {
                                    if(!actionSurface.isMoving)
                                    {
                                        ObjectIsInteractable();
                                    }
                                    else
                                    {
                                        ObjectIsNotInteractable();
                                    }
                                }
                                else
                                {
                                    ObjectIsNotInteractable();
                                }
                            }
                        }
                        else if(customSurface)
                        {
                            if(isHoldingAnObject)
                            {
                                if(customSurface.hasAnObject)
                                {
                                    ObjectIsNotInteractable();
                                }
                                else
                                {
                                    ObjectIsInteractable();
                                }
                            }
                            else
                            {
                                ObjectIsNotInteractable();
                            }
                        }
                        else if(viewableObject)
                        {
                            if(!viewableObject.interactable)
                            {
                                if(!viewableObject.interactionInProgress)
                                {
                                    if(!viewableObject.objectDisabled)
                                    {
                                        ObjectIsInteractable();
                                    }
                                    else
                                    {
                                        ObjectIsNotInteractable();
                                    }
                                }
                                else
                                {
                                    ObjectIsNotInteractable();
                                }
                            }
                            else
                            {
                                ObjectIsInteractable();
                            }
                        }
                        else
                        {
                            if(!lockInteractionReticle)
                            {
                                ObjectIsNotInteractable();
                            }
                            else
                            {
                                ObjectIsInteractable();
                            }
                        }
                    }
                    else
                    {
                        if(!lockInteractionReticle)
                        {
                            ObjectIsNotInteractable();
                        }
                        else
                        {
                            ObjectIsInteractable();
                        }
                    }

                #endregion

                #region Performing Object Action

                    if(InputHandler.instance.cameraInputData.clicked)
                    {
                        if(!objectIsFurtherAway)
                        {
                            if(interactableObject)
                            {
                                if(!interactableObject.isMoving && !interactableObject.controlledObjectIsMoving)
                                {
                                    if(!interactableObject.hasTrigger)
                                    {
                                        InteractableObjectAction();
                                    }
                                    else
                                    {
                                        if(interactableObject.enteredTrigger)
                                        {
                                            InteractableObjectAction();
                                        }
                                    }
                                }
                            }
                            else if(pickableObject)
                            {
                                if(!isHoldingAnObject)
                                {
                                    pickableObject.PickUpObject(true, false, false);
                                    pickableObject.ShowIcon();
                                    isHoldingAnObject = true;

                                    scriptOfObjectPlayerIsHolding = hit.transform.GetComponent<PickableObject>();

                                    pickableObject.isResting = false;
                                }
                                else if(scriptOfObjectPlayerIsHolding.shoppingBasket)
                                {
                                    if(scriptOfObjectPlayerIsHolding.basket.ItemCheck(pickableObject, true))
                                    {
                                       pickableObject.PickUpObject(true, false, true);
                                       pickableObject.isResting = false;
                                       scriptOfObjectPlayerIsHolding.basket.ChangeItemCount();
                                    }
                                }
                            }   
                            else if(flatSurface)
                            {
                                if(isHoldingAnObject)
                                {
                                    if(!flatSurface.hasAnObject)
                                    {
                                        scriptOfObjectPlayerIsHolding.PutDownObject(flatSurface, false);
                                        scriptOfObjectPlayerIsHolding.HideIcon();
                                        isHoldingAnObject = false;

                                        flatSurface.hasAnObject = true;
                                        scriptOfObjectPlayerIsHolding.isResting = true;
                                        flatSurface.objectOnSurface = scriptOfObjectPlayerIsHolding;
                                        scriptOfObjectPlayerIsHolding = null;
                                    }
                                } 
                            }
                            else if(actionSurface)
                            {
                                if(isHoldingAnObject)
                                {
                                    if(!actionSurface.hasAnObject)
                                    {
                                        if(actionSurface.RequiredObject(scriptOfObjectPlayerIsHolding))
                                        {
                                            if(scriptOfObjectPlayerIsHolding == actionSurface.masterKey)
                                            {
                                                actionSurface.isMasterKey = true;
                                                scriptOfObjectPlayerIsHolding.UseObject(actionSurface.actionSurfaceMasterKey);
                                                scriptOfObjectPlayerIsHolding.actionSurfaceMasterKey = actionSurface.actionSurfaceMasterKey;
                                            }
                                            else
                                            {
                                                scriptOfObjectPlayerIsHolding.UseObject(null);
                                            }

                                            actionSurface.objectOnActionSurface = scriptOfObjectPlayerIsHolding;
                                            scriptOfObjectPlayerIsHolding.HideIcon();
                                            scriptOfObjectPlayerIsHolding.playerIsHoldingObject = false;
                                            isHoldingAnObject = false;

                                            actionSurface.hasAnObject = true; 
                                        }
                                    }
                                }
                                else
                                {
                                    if(actionSurface.hasAnObject)
                                    {
                                        if(!actionSurface.isMoving)
                                        {
                                            if(actionSurface.isMasterKey)
                                            {
                                                scriptOfObjectPlayerIsHolding = actionSurface.masterKey;
                                                scriptOfObjectPlayerIsHolding.actionSurfaceMasterKey = null;
                                                actionSurface.isMasterKey = false;
                                                actionSurface.masterKey.PickUpObjectFromActionSurface();
                                                actionSurface.objectOnActionSurface = null;
                                                scriptOfObjectPlayerIsHolding.playerIsHoldingObject = true;
                                                actionSurface.masterKey.ShowIcon();
                                            }
                                            else
                                            {
                                                scriptOfObjectPlayerIsHolding = actionSurface.requiredObject;
                                                actionSurface.requiredObject.PickUpObjectFromActionSurface();
                                                scriptOfObjectPlayerIsHolding.playerIsHoldingObject = true;
                                                actionSurface.requiredObject.ShowIcon();
                                            }

                                            isHoldingAnObject = true;

                                            actionSurface.hasAnObject = false;
                                        }
                                    }
                                }
                            }
                            else if(customSurface)
                            {
                                if(isHoldingAnObject)
                                {
                                    if(!customSurface.hasAnObject)
                                    {
                                        if(customSurface.RequiredObject(scriptOfObjectPlayerIsHolding.gameObject))
                                        {
                                            scriptOfObjectPlayerIsHolding.HangObject(customSurface);
                                            scriptOfObjectPlayerIsHolding.HideIcon();
                                            isHoldingAnObject = false;

                                            customSurface.hasAnObject = true;
                                            scriptOfObjectPlayerIsHolding.isResting = true;
                                            customSurface.objectOnSurface = scriptOfObjectPlayerIsHolding;
                                        }
                                    }
                                }
                            }
                            else if(viewableObject)
                            {
                                if(!viewableObject.interactionInProgress)
                                {
                                    if(!viewableObject.objectDisabled)
                                    {
                                        viewableObject.ViewObject(hit.distance);
                                    }
                                }
                            }
                            else if(ladder)
                            {
                                ladder.ClimbingCheck();
                            }
                        }
                    }
                    else if(ladder)
                    {
                        ladder.ClimbingCheck();
                    }

                #endregion
            }
        }

    #endregion

    #region Custom Methods

        private void ObjectIsInteractable()
        {
            ReticleController.instance.ShowIcon(true);
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
        }

        private void ObjectIsNotInteractable()
        {
            ReticleController.instance.ShowIcon(false);
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * longInteractionRange, Color.red);
        }

        private void InteractableObjectCheck()
        {
            if(interactableObject.isLocked)
            {
                if(interactableObject.lockedAnimation)
                {
                    ObjectIsInteractable();
                }
                else
                {
                    ObjectIsNotInteractable();
                }
            }
            else
            {
                if(interactableObject.interactableOnlyOnce)
                {
                    if(interactableObject.performedAction)
                    {
                        ObjectIsNotInteractable();
                    }
                    else
                    {
                        ObjectIsInteractable();
                    }
                }
                else
                {
                    ObjectIsInteractable();
                }
            }  
        }

        private void InteractableObjectAction()
        {
            if(!interactableObject.isLocked)
            {
                if(interactableObject.interactableOnlyOnce)
                {
                    if(!interactableObject.performedAction)
                    {
                        interactableObject.PerformAction(false);
                    }
                }
                else
                {
                    interactableObject.PerformAction(false);
                }
            }
            else
            {
                if(interactableObject.lockedAnimation)
                {
                    interactableObject.PlayLockedAnimation();
                }
            }
        }

    #endregion
}