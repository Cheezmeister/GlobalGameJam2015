﻿using UnityEditor.AnimatedValues;
using UnityEngine;

namespace FullInspector.Modules.Common {
    public class ObjectFoldoutStateGraphMetadata : IGraphMetadataItem {
        /// <summary>
        /// Is the foldout currently active, ie, is the rendered item being displayed or is the
        /// short-form foldout being displayed?
        /// </summary>
        public bool IsActive {
            get {
                return _isActive.value;
            }
            set {
                if (value != _isActive.target) {
                    _isActive.target = value;
                }
            }
        }

        /// <summary>
        /// What percentage are we at in the animation between active states?
        /// </summary>
        public float AnimPercentage {
            get {
                return _isActive.faded;
            }
        }
        [SerializeField]
        private AnimBool _isActive = new AnimBool(false);

        /// <summary>
        /// Are we currently animating between different states?
        /// </summary>
        public bool IsAnimating {
            get {
                return _isActive.isAnimating;
            }
        }
    }
}