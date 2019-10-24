// Lic:
// Neg
// Negative
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.10.24
// EndLic
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using UseJCR6;
using TrickyUnits;

namespace Neg {



    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TJCRDIR JCR;
        TJCRCreate JCRC;
        string[] args => Environment.GetCommandLineArgs();
        TJCREntry[] Files;

        TQMGImage Original;
        TQMGImage Negative;

        int FilesIDX = 0;
        int FilesCNT => Files.Length;

        void Assert(bool condition, string errormessage) {
            if (!condition) throw new Exception(errormessage);
            new JCR6_lzma();
            new JCR6_zlib();
            new JCR6_jxsrcca();
            new JCR6_RealDir();
            new JCR6_WAD();
            new JCR_QuakePack();
        }

        void Assert(string c, string e) => Assert(c != "", e);
        void Assert(int i, string e) => Assert(i != 0, e);
        void Assert(object o, string e) => Assert(o != null, e);

        void GetJCR() {

            var f = new List<TJCREntry>();

            JCR = JCR6.Dir(Dirry.AD(args[1]));
            Assert(JCR, JCR6.JERROR);

            foreach(TJCREntry e in JCR.Entries.Values) {
                var ok = qstr.Suffixed(e.Entry.ToLower(), ".png");
                var p = false;
                for(int i = 2; i < args.Length; i++) {
                    p = p || qstr.Prefixed(e.Entry.ToLower(), args[i].ToLower());
                }
                ok = (args.Length == 2 || p);
                if (ok) f.Add(e);
            }
            Files = f.ToArray();
            TQMG.Init(graphics, GraphicsDevice, spriteBatch, JCR);
            JCRC = new TJCRCreate($"{qstr.StripExt(args[1])}.Negative.JCR");
        }

        void Neg() {
            Negative = TQMG.NewImage(Original.Width, Original.Height);
            for (int y=0;y<Original.Height;y++) for (int x = 0; x < Original.Width; x++) {
                    var Pix = Original.GetPixel(x, y);
                    Negative.PutPixel(x, y, (byte)(255 - Pix.R), (byte)(255 - Pix.G), (byte)(255 - Pix.B), Pix.A);
                }
        }



        /* Crappy Callback Stuff */
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Dirry.InitAltDrives();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Assert(args.Length > 1, "Stuff needed!");
            GetJCR();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            if (FilesIDX >= FilesCNT) {
                JCRC.Close();
                Exit();
            } else {
                Original = TQMG.GetImage(Files[FilesIDX].Entry);
                Neg();
                Negative.Save(JCRC, $"{qstr.StripExt(Files[FilesIDX].Entry)}.Negative.png", "lzma", Files[FilesIDX].Author, Files[FilesIDX].Notes);
                FilesIDX++;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Fuchsia);
            if (Original != null) Original.Draw(2, 2);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

