using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class SoundManager
    {
        private readonly ActionQueue actionQueue;
        private readonly SoundPlayer teleport;
        private readonly SoundPlayer sweetEaten;
        private readonly SoundPlayer gameOver;
        public SoundManager()
        {
            this.actionQueue = new ActionQueue();

            teleport = new SoundPlayer(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\sounds\teleport2.wav");
            teleport.LoadAsync();

            sweetEaten = new SoundPlayer(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\sounds\pacman_eatfruit.wav");
            sweetEaten.LoadAsync();

            gameOver = new SoundPlayer(@"C:\Users\david.betteridge\Documents\GitHub\PuzzleGame\PuzzleUI\sounds\pacman_death.wav");
            gameOver.LoadAsync();
        }
        public void PlaySound(string soundType)
        {
            this.actionQueue.Add(() =>
            {
                switch (soundType)
                {
                    case "SWEET_EATEN":
                        sweetEaten.PlaySync();
                        break;
                    case "TELEPORT":
                        teleport.PlaySync();
                        break;
                    case "GAME_OVER":
                        gameOver.PlaySync();
                        break;
                    default:
                        break;
                }

            });
        }
    }
}
