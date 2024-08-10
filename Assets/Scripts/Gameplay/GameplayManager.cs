using System;
using RuneForger.Character;
using RuneForger.UI;

namespace RuneForger.Gameplay
{
    public class GameplayManager : SingletonPersistent<GameplayManager>
    {
        public GameCharacter PlayerCharacter {get; set;}
        public PCController PCController {get; private set;}
        public CharacterInteract CharacterInteract {get; set;}
        public InteractLabel InteractLabel {get; private set;}

        protected override void Awake()
        {
            PlayerCharacter = FindAnyObjectByType<GameCharacter>();
            CharacterInteract = PlayerCharacter.GetComponent<CharacterInteract>();
            PCController = FindAnyObjectByType<PCController>();
            InteractLabel = FindAnyObjectByType<InteractLabel>();
        }
    }
}