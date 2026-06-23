using System.Collections.Generic;

using Assets.Scripts.Core;

using UnityEngine;

namespace Assets.Scripts.DevTools.RuntimeDebugger
{
    public class ChainDebugVisualizer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showChainColors = true;
        [SerializeField] private float highlightAlpha = 0.5f;

        private List<GameObject> _chainHighlights = new();

        private void OnEnable()
        {
            ConveyorChainManager.ChainsDetected += OnChainsDetected;
        }

        private void OnDisable()
        {
            ConveyorChainManager.ChainsDetected -= OnChainsDetected;
            ClearHighlights();
        }

        private void OnChainsDetected(List<ConveyorChain> chains)
        {
            ClearHighlights();

            if(!showChainColors) return;

            for(int chainIndex = 0; chainIndex < chains.Count; chainIndex++)
            {
                float hue = chainIndex / (float)Mathf.Max(chains.Count, 1);
                Color chainColor = Color.HSVToRGB(hue, 0.8f, 0.9f);
                chainColor.a = highlightAlpha;

                foreach(Vector3Int position in chains[chainIndex].Positions)
                {
                    _chainHighlights.Add(CreateHighlight(position, chainColor));
                }
            }
        }

        private GameObject CreateHighlight(Vector3Int position, Color color)
        {
            GameObject highlight = new GameObject("ChainHighlight");
            SpriteRenderer spriteRenderer = highlight.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("sprites/white_square");
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = 50;

            highlight.transform.position = WorldManager.Instance.CellToWorld(position);
            highlight.transform.localScale = Vector3.one;
            return highlight;
        }

        private void ClearHighlights()
        {
            foreach(GameObject highlight in _chainHighlights)
            {
                if(highlight != null) Destroy(highlight);
            }
            _chainHighlights.Clear();
        }
    }
}
