﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Tests.Visual.Platform
{
    public class TestSceneSafeArea : FrameworkTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(SafeAreaSnappingContainer),
            typeof(SafeAreaTargetContainer),
            typeof(EdgeSnappingContainer),
            typeof(SnapTargetContainer)
        };

        private readonly Bindable<MarginPadding> safeAreaPadding = new BindableMarginPadding();

        private readonly Box safeAreaTopOverlay;
        private readonly Box safeAreaBottomOverlay;
        private readonly Box safeAreaLeftOverlay;
        private readonly Box safeAreaRightOverlay;

        public TestSceneSafeArea()
        {
            var snappingContainer = new SafeAreaSnappingContainer
            {
                Position = new Vector2(50, 50),
                Size = new Vector2(400, 300),
                SafeEdges = Edges.None,
                Child = new Box
                {
                    Colour = Color4.Blue,
                    RelativeSizeAxes = Axes.Both
                }
            };

            Children = new Drawable[]
            {
                new MarginPaddingControlsContainer(snappingContainer, safeAreaPadding)
                {
                    Position = new Vector2(20, 20),
                    AutoSizeAxes = Axes.Both,
                },
                new SafeAreaTargetContainer
                {
                    Position = new Vector2(20, 280),
                    Size = new Vector2(500, 400),
                    SafeAreaPadding = safeAreaPadding,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = Color4.Red,
                            RelativeSizeAxes = Axes.Both,
                        },
                        snappingContainer,
                        safeAreaLeftOverlay = new Box
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Width = 0,
                            RelativeSizeAxes = Axes.Y,
                            Colour = Color4.Green,
                            Alpha = 0.2f
                        },
                        safeAreaRightOverlay = new Box
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Width = 0,
                            RelativeSizeAxes = Axes.Y,
                            Colour = Color4.Green,
                            Alpha = 0.2f
                        },
                        safeAreaTopOverlay = new Box
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Height = 0,
                            RelativeSizeAxes = Axes.X,
                            Colour = Color4.Green,
                            Alpha = 0.2f
                        },
                        safeAreaBottomOverlay = new Box
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Height = 0,
                            RelativeSizeAxes = Axes.X,
                            Colour = Color4.Green,
                            Alpha = 0.2f
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            safeAreaPadding.ValueChanged += e => safeAreaPaddingChanged(e.NewValue);
        }

        private void safeAreaPaddingChanged(MarginPadding padding)
        {
            safeAreaLeftOverlay.Width = padding.Left;
            safeAreaRightOverlay.Width = padding.Right;
            safeAreaTopOverlay.Height = padding.Top;
            safeAreaBottomOverlay.Height = padding.Bottom;
        }

        private class MarginPaddingControlsContainer : FillFlowContainer
        {
            private readonly Bindable<float> safeAreaPaddingTop;
            private readonly Bindable<float> safeAreaPaddingBottom;
            private readonly Bindable<float> safeAreaPaddingLeft;
            private readonly Bindable<float> safeAreaPaddingRight;

            private readonly Bindable<MarginPadding> bindableMarginPadding;

            public MarginPaddingControlsContainer(SafeAreaSnappingContainer snappingContainer, Bindable<MarginPadding> bindableMarginPadding)
            {
                this.bindableMarginPadding = bindableMarginPadding;

                safeAreaPaddingTop = new BindableFloat { MinValue = 0, MaxValue = 200 };
                safeAreaPaddingBottom = new BindableFloat { MinValue = 0, MaxValue = 200 };
                safeAreaPaddingLeft = new BindableFloat { MinValue = 0, MaxValue = 200 };
                safeAreaPaddingRight = new BindableFloat { MinValue = 0, MaxValue = 200 };

                Direction = FillDirection.Vertical;
                Spacing = new Vector2(10);
                Children = new Drawable[]
                {
                    new MarginPaddingControl(snappingContainer, "Top", safeAreaPaddingTop, Edges.Top),
                    new MarginPaddingControl(snappingContainer, "Bottom", safeAreaPaddingBottom, Edges.Bottom),
                    new MarginPaddingControl(snappingContainer, "Left", safeAreaPaddingLeft, Edges.Left),
                    new MarginPaddingControl(snappingContainer, "Right", safeAreaPaddingRight, Edges.Right),
                };

                safeAreaPaddingTop.ValueChanged += updateMarginPadding;
                safeAreaPaddingBottom.ValueChanged += updateMarginPadding;
                safeAreaPaddingLeft.ValueChanged += updateMarginPadding;
                safeAreaPaddingRight.ValueChanged += updateMarginPadding;
            }

            private void updateMarginPadding(ValueChangedEvent<float> e)
            {
                bindableMarginPadding.Value = new MarginPadding
                {
                    Top = safeAreaPaddingTop.Value,
                    Bottom = safeAreaPaddingBottom.Value,
                    Left = safeAreaPaddingLeft.Value,
                    Right = safeAreaPaddingRight.Value,
                };
            }

            private class MarginPaddingControl : FillFlowContainer
            {
                public MarginPaddingControl(SafeAreaSnappingContainer snappingContainer, string title, Bindable<float> bindable, Edges edge)
                {
                    SpriteText valueText;
                    EdgeModeDropdown dropdown;

                    Direction = FillDirection.Horizontal;
                    Spacing = new Vector2(10, 0);
                    AutoSizeAxes = Axes.Both;

                    Children = new Drawable[]
                    {
                        new SpriteText { Text = title, Width = 60 },
                        valueText = new SpriteText { Width = 50 },
                        new BasicSliderBar<float>
                        {
                            Current = bindable,
                            Size = new Vector2(100, 20),
                        },
                        dropdown = new EdgeModeDropdown(),
                    };

                    dropdown.Current.ValueChanged += e =>
                    {
                        switch (e.NewValue)
                        {
                            case EdgeModes.None:
                                snappingContainer.SnappedEdges &= ~edge;
                                snappingContainer.SafeEdges &= ~edge;
                                break;

                            case EdgeModes.Snapped:
                                snappingContainer.SnappedEdges |= edge;
                                snappingContainer.SafeEdges &= ~edge;
                                break;

                            case EdgeModes.Safe:
                                snappingContainer.SnappedEdges &= ~edge;
                                snappingContainer.SafeEdges |= edge;
                                break;
                        }
                    };

                    bindable.ValueChanged += e => valueText.Text = $"{e.NewValue:F1}";

                    bindable.TriggerChange();
                }
            }

            private class EdgeModeDropdown : BasicDropdown<EdgeModes>
            {
                public EdgeModeDropdown()
                {
                    Items = new[] { EdgeModes.None, EdgeModes.Snapped, EdgeModes.Safe };
                    Width = 100;
                    Current.Value = EdgeModes.None;
                }
            }

            private enum EdgeModes
            {
                None,
                Snapped,
                Safe,
            }
        }
    }
}
