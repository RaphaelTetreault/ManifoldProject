using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools
{
    [ExecuteInEditMode]
    public abstract class EditableDependantMonoBehaviour<TEditableComponent> : MonoBehaviour
        where TEditableComponent : Component, IEditableComponent<TEditableComponent>
    {
        [SerializeField]
        private TEditableComponent editableComponent;

        public TEditableComponent EditableComponent => editableComponent;


        public virtual void OnEnable()
        {
            if (editableComponent)
            {
                editableComponent.OnEdited += OnEdited;
            }
        }

        public virtual void OnDisable()
        {
            if (editableComponent)
            {
                editableComponent.OnEdited -= OnEdited;
            }
        }

        public virtual void OnValidate()
        {
            if (editableComponent != null)
            {
                editableComponent = GetComponent<TEditableComponent>();

                // If not null now
                if (editableComponent != null)
                {
                    OnEnable();
                }
            }
        }

        public abstract void OnEdited(TEditableComponent value);
    }
}
