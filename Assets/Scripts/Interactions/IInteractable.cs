using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Interactions
{
    public interface IInteractable
    {
        abstract void Interact();
        abstract bool CanInteract();
    }
}
