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
            PlayerCharacter = GameObject.FindWithTag("Player").GetComponent<GameCharacter>();
            Status = PlayerCharacter.GetComponent<CharacterStatus>();
            CharacterInteract = PlayerCharacter.GetComponent<CharacterInteract>();
            
            ForceFieldEmitter = FindAnyObjectByType<ForceFieldEmitter>();
            
            PCController = FindAnyObjectByType<PCController>();
            ViewCamera = PCController.ViewCamera;
            InteractLabel = FindAnyObjectByType<InteractLabel>();
        }
    }
}