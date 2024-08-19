using System;
using RuneForger.Character;
using RuneForger.Player;
using RuneForger.UI;
using UnityEngine;

namespace RuneForger.Gameplay
{
    public class GameplayManager : SingletonPersistent<GameplayManager>
    {
        public GameCharacter PlayerCharacter {get; private set;}
        public CharacterStatus Status { get; private set; }
        public ForceFieldEmitter ForceFieldEmitter { get; private set; }
        public PCController PCController {get; private set;}
        public CharacterInteract CharacterInteract {get; set;}
        public InteractLabel InteractLabel {get; private set;}
        public Camera ViewCamera { get; private set; }

        protected override void Awake()
        {
            PlayerCharacter = FindAnyObjectByType<GameCharacter>();
            Status = PlayerCharacter.GetComponent<CharacterStatus>();
            ForceFieldEmitter = FindAnyObjectByType<ForceFieldEmitter>();
            
            CharacterInteract = PlayerCharacter.GetComponent<CharacterInteract>();
            PCController = FindAnyObjectByType<PCController>();
            ViewCamera = PCController.ViewCamera;
            InteractLabel = FindAnyObjectByType<InteractLabel>();
        }
    }
}