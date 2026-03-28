using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace Alperen.Scripts.Interaction
{
    /// <summary>
    /// Filter that only allows interactables with the "DinoBone" tag to be selected.
    /// </summary>
    public class DinoBoneTagFilter : IXRTargetFilter
    {
        [SerializeField] private string acceptedTag = "DinoBone";

        /// <summary>
        /// Whether this Target Filter can process and filter targets.
        /// </summary>
        public bool canProcess => true;

        /// <summary>
        /// Called by Unity when given Interactor links to this filter.
        /// </summary>
        /// <param name="interactor">The Interactor being linked to this filter.</param>
        public void Link(IXRInteractor interactor)
        {
            // Empty - not implemented
        }

        /// <summary>
        /// Called by Unity when given Interactor unlinks from this filter.
        /// </summary>
        /// <param name="interactor">The Interactor being unlinked from this filter.</param>
        public void Unlink(IXRInteractor interactor)
        {
            // Empty - not implemented
        }

        /// <summary>
        /// Called by linked Interactor to filter the Interactables that it could possibly interact with this frame.
        /// Only allows interactables with the "DinoBone" tag.
        /// </summary>
        /// <param name="interactor">The linked Interactor whose Interactable candidates are being filtered.</param>
        /// <param name="targets">The read only list of candidate Interactables to filter.</param>
        /// <param name="results">The results list to populate with filtered results.</param>
        public void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
        {
            // Empty - not implemented, filter logic handled by IXRTargetFilter property
        }
    }
}
