//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.1
//     from Assets/Input/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Pause"",
            ""id"": ""28ab8866-a0dc-4951-9647-4489288076d9"",
            ""actions"": [
                {
                    ""name"": ""PauseAction"",
                    ""type"": ""Button"",
                    ""id"": ""0cb36920-d0ac-46f3-a1d0-351fbe82427f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b2c5250e-a660-4dc1-bc19-c907e60e8ffc"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player"",
            ""id"": ""e46362e4-df9e-47f5-9ac3-ca18a3dd3154"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""9a008c45-096e-4cba-866c-c9cad56d3083"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ce2c912f-6fb0-491f-9c68-4299edc19fb9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Place Trap"",
                    ""type"": ""Button"",
                    ""id"": ""e1376b3a-294c-4275-a518-15da36561a08"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SetTrap1"",
                    ""type"": ""Button"",
                    ""id"": ""305dacb5-ed5b-4481-8288-e8545653e14d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SetTrap2"",
                    ""type"": ""Button"",
                    ""id"": ""c2643fb8-7d4f-49b3-b78b-a0863b323b31"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SetTrap3"",
                    ""type"": ""Button"",
                    ""id"": ""f1219c89-a821-4b7a-929a-271c65ddbd04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""eba80a05-b2f3-45fd-89bf-c6afacb1bacb"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""dbcd96df-39e9-4d3a-81e9-c1cfa05f8b47"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a4ece0ae-7f7c-49a1-94ce-8be51b01c993"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a8a3d484-7d87-4845-8990-82792412cf28"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""5f77fc80-20f8-42fb-b8b2-732e28879b2f"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1cc835e0-618b-4af0-b43b-c37ff9cdb10c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b9136ec-6f26-43ce-a461-9e4dd11114f8"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""988bf463-ff70-47f1-9463-af4558b8c501"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Place Trap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d6d22d8-6090-45e2-80a8-6dc1d73d9b3f"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetTrap1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f1097bb-af4b-466d-a626-56d67a45262a"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetTrap2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""292f8c6a-f504-480c-9977-edf7e692f23c"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SetTrap3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""1363d107-8a52-4c30-a508-dbb061969a1b"",
            ""actions"": [
                {
                    ""name"": ""Advance Dialogue"",
                    ""type"": ""Button"",
                    ""id"": ""671b8445-8ad3-4fb0-ace6-6fea0e80a433"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skip Dialogue"",
                    ""type"": ""Button"",
                    ""id"": ""1ac75756-cf99-40c3-a019-bf252bc578a2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=1)"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9e5cb03e-42cc-46f3-8896-67ca887f023d"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Advance Dialogue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2209c1dc-372f-4540-91b1-7abbed5b05ac"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skip Dialogue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Pause
        m_Pause = asset.FindActionMap("Pause", throwIfNotFound: true);
        m_Pause_PauseAction = m_Pause.FindAction("PauseAction", throwIfNotFound: true);
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_PlaceTrap = m_Player.FindAction("Place Trap", throwIfNotFound: true);
        m_Player_SetTrap1 = m_Player.FindAction("SetTrap1", throwIfNotFound: true);
        m_Player_SetTrap2 = m_Player.FindAction("SetTrap2", throwIfNotFound: true);
        m_Player_SetTrap3 = m_Player.FindAction("SetTrap3", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_AdvanceDialogue = m_UI.FindAction("Advance Dialogue", throwIfNotFound: true);
        m_UI_SkipDialogue = m_UI.FindAction("Skip Dialogue", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Pause
    private readonly InputActionMap m_Pause;
    private List<IPauseActions> m_PauseActionsCallbackInterfaces = new List<IPauseActions>();
    private readonly InputAction m_Pause_PauseAction;
    public struct PauseActions
    {
        private @PlayerInputActions m_Wrapper;
        public PauseActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseAction => m_Wrapper.m_Pause_PauseAction;
        public InputActionMap Get() { return m_Wrapper.m_Pause; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseActions set) { return set.Get(); }
        public void AddCallbacks(IPauseActions instance)
        {
            if (instance == null || m_Wrapper.m_PauseActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PauseActionsCallbackInterfaces.Add(instance);
            @PauseAction.started += instance.OnPauseAction;
            @PauseAction.performed += instance.OnPauseAction;
            @PauseAction.canceled += instance.OnPauseAction;
        }

        private void UnregisterCallbacks(IPauseActions instance)
        {
            @PauseAction.started -= instance.OnPauseAction;
            @PauseAction.performed -= instance.OnPauseAction;
            @PauseAction.canceled -= instance.OnPauseAction;
        }

        public void RemoveCallbacks(IPauseActions instance)
        {
            if (m_Wrapper.m_PauseActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPauseActions instance)
        {
            foreach (var item in m_Wrapper.m_PauseActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PauseActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PauseActions @Pause => new PauseActions(this);

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_PlaceTrap;
    private readonly InputAction m_Player_SetTrap1;
    private readonly InputAction m_Player_SetTrap2;
    private readonly InputAction m_Player_SetTrap3;
    public struct PlayerActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @PlaceTrap => m_Wrapper.m_Player_PlaceTrap;
        public InputAction @SetTrap1 => m_Wrapper.m_Player_SetTrap1;
        public InputAction @SetTrap2 => m_Wrapper.m_Player_SetTrap2;
        public InputAction @SetTrap3 => m_Wrapper.m_Player_SetTrap3;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @PlaceTrap.started += instance.OnPlaceTrap;
            @PlaceTrap.performed += instance.OnPlaceTrap;
            @PlaceTrap.canceled += instance.OnPlaceTrap;
            @SetTrap1.started += instance.OnSetTrap1;
            @SetTrap1.performed += instance.OnSetTrap1;
            @SetTrap1.canceled += instance.OnSetTrap1;
            @SetTrap2.started += instance.OnSetTrap2;
            @SetTrap2.performed += instance.OnSetTrap2;
            @SetTrap2.canceled += instance.OnSetTrap2;
            @SetTrap3.started += instance.OnSetTrap3;
            @SetTrap3.performed += instance.OnSetTrap3;
            @SetTrap3.canceled += instance.OnSetTrap3;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @PlaceTrap.started -= instance.OnPlaceTrap;
            @PlaceTrap.performed -= instance.OnPlaceTrap;
            @PlaceTrap.canceled -= instance.OnPlaceTrap;
            @SetTrap1.started -= instance.OnSetTrap1;
            @SetTrap1.performed -= instance.OnSetTrap1;
            @SetTrap1.canceled -= instance.OnSetTrap1;
            @SetTrap2.started -= instance.OnSetTrap2;
            @SetTrap2.performed -= instance.OnSetTrap2;
            @SetTrap2.canceled -= instance.OnSetTrap2;
            @SetTrap3.started -= instance.OnSetTrap3;
            @SetTrap3.performed -= instance.OnSetTrap3;
            @SetTrap3.canceled -= instance.OnSetTrap3;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_AdvanceDialogue;
    private readonly InputAction m_UI_SkipDialogue;
    public struct UIActions
    {
        private @PlayerInputActions m_Wrapper;
        public UIActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @AdvanceDialogue => m_Wrapper.m_UI_AdvanceDialogue;
        public InputAction @SkipDialogue => m_Wrapper.m_UI_SkipDialogue;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @AdvanceDialogue.started += instance.OnAdvanceDialogue;
            @AdvanceDialogue.performed += instance.OnAdvanceDialogue;
            @AdvanceDialogue.canceled += instance.OnAdvanceDialogue;
            @SkipDialogue.started += instance.OnSkipDialogue;
            @SkipDialogue.performed += instance.OnSkipDialogue;
            @SkipDialogue.canceled += instance.OnSkipDialogue;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @AdvanceDialogue.started -= instance.OnAdvanceDialogue;
            @AdvanceDialogue.performed -= instance.OnAdvanceDialogue;
            @AdvanceDialogue.canceled -= instance.OnAdvanceDialogue;
            @SkipDialogue.started -= instance.OnSkipDialogue;
            @SkipDialogue.performed -= instance.OnSkipDialogue;
            @SkipDialogue.canceled -= instance.OnSkipDialogue;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface IPauseActions
    {
        void OnPauseAction(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnPlaceTrap(InputAction.CallbackContext context);
        void OnSetTrap1(InputAction.CallbackContext context);
        void OnSetTrap2(InputAction.CallbackContext context);
        void OnSetTrap3(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnAdvanceDialogue(InputAction.CallbackContext context);
        void OnSkipDialogue(InputAction.CallbackContext context);
    }
}
