using System.Collections;
using Client.UI.Scripts;
using Code.Core.Client.UI.Scripts;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.UnityExtensions.Independent;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces
{
    public class SpellDescription : UIInterface<SpellDescription>
    {
        public Icon Icon;
        public tk2dTextMesh TitleLabel,DescriptionLabel;

        [SerializeField] private tk2dSlicedSprite _background;
        
        internal static void Show(Spell spell, Vector3 vector3)
        {
            I.Show();

            I.Spell = spell;
            I.transform.position = vector3;

            I.Icon.Texture = spell.Icon;

            I.TitleLabel.text = spell.name;
            I.TitleLabel.ForceBuild();

            I.DescriptionLabel.text = spell.Description;
            I.DescriptionLabel.ForceBuild();

        }

        public Spell Spell { get; set; }

        IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(5f);
            Hide();
        }

        public override void Hide()
        {
            Visible = false;
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.zero,
                    delegate(Vector3 vector3)
                    {
                        if (!Visible)
                        transform.localScale = vector3;
                    },
                    delegate
                    {
                        if (!Visible)
                        gameObject.SetActive(false);
                    },
                    0.33f
                    )
                );

        }

        public override void Show()
        {
            Visible = true;
            gameObject.SetActive(true);
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one,
                    delegate(Vector3 vector3)
                    {
                        if (Visible)
                        transform.localScale = vector3;
                    },
                    delegate
                    {
                        if (Visible)
                        {
                            gameObject.SetActive(true);
                        }
                    },
                    0.33f
                    )
                );
        }
    }
}

