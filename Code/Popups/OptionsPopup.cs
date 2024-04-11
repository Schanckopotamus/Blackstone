using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.Menus
{
    public partial class OptionsPopup : Control
    {
        private int _defaultAnte = 1;
        private Label _anteValueLabel;
        private Button _saveButton;
        private GameMetadata _gameMetadata;
        private HSlider _hSlider;

        // Set the Game Ante
        public int Ante
        {
            get
            {
                return int.TryParse(_anteValueLabel.Text, out int ante)
                        ? ante
                        : 1;
            }
            set
            {
                _anteValueLabel.Text = value.ToString();
            }
        }

        public override void _Ready()
        {
            _gameMetadata = GetNode<GameMetadata>("/root/GameMetadata");
            _hSlider = GetNode<HSlider>("PopupPanel/VBoxContainer/HSlider");
            _anteValueLabel = GetNode<Label>("PopupPanel/VBoxContainer/AnteValueLabel");
            _saveButton = GetNode<Button>("PopupPanel/VBoxContainer/SaveButton");

            _anteValueLabel.Text = GetAnteValueForInitialization();
        }

        public void SliderValueChanged(float value)
        {
            Ante = (int)value;
        }

        private string GetAnteValueForInitialization()
        {
            if (_gameMetadata.GameAnte == _defaultAnte)
            {
                return _defaultAnte.ToString();
            }

            Ante = _gameMetadata.GameAnte;
            _hSlider.SetValueNoSignal(Ante);
            _hSlider.Value = Ante;
            return _gameMetadata.GameAnte.ToString();
        }

        private void OnButtonPressed()
        {
            _gameMetadata.SetGameAnte(Ante);
            this.QueueFree();
        }
    }
}
