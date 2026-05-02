using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class Ticket
    {
        public byte customerId;
    }
    public class TicketObject
    {
        public Ticket ticket;
        private static readonly Vector2 initialPosition = new Vector2(43, 4);
        public Vector2 position = new Vector2(0, 0);
        private static Texture2D? gfxTicket = null;
        private static Texture2D? gfxTicket1 = null;
        private static Texture2D? gfxTicket2 = null;
        private static Color ticketColor = Color.White;

        public TicketObject(Ticket ticket)
        {
            this.ticket = ticket;
        }
        public void Initialize()
        {
            position = initialPosition;
        }
        public bool Update(GameTime gameTime, bool handleInput)
        {
            return false;
        }
        public void DrawLayer1(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicket2 == null)
                return;
            spriteBatch.Draw(gfxTicket2, GlobalGraphics.Scale(new Rectangle(43 + gfxTicket1.Bounds.Size.X, 4, gfxTicket2.Bounds.Width, gfxTicket2.Bounds.Height)), ticketColor);
        }
        public void DrawLayer2(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicket == null
                || gfxTicket1 == null)
                return;
            spriteBatch.Draw(gfxTicket1, GlobalGraphics.Scale(new Rectangle(43, 4, gfxTicket1.Bounds.Width, gfxTicket1.Bounds.Height)), ticketColor);
            spriteBatch.Draw(gfxTicket, GlobalGraphics.Scale(new Rectangle(43, 12, gfxTicket.Bounds.Width, gfxTicket.Bounds.Height)), ticketColor);
        }
        public static void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            gfxTicket = GlobalContent.AddTexture("Ticket", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket"));
            gfxTicket1 = GlobalContent.AddTexture("Ticket1", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket1"));
            gfxTicket2 = GlobalContent.AddTexture("Ticket2", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket2"));
            ticketColor = ThemeManager.GetColor("OrderStationTicket");
        }
    }
    public class TicketScreen : IScreen {
        public string title { get; set; } = "Tickets";
        public int layer { get; set; } = 4;
        public List<TicketObject> tickets = new List<TicketObject>();
        private Texture2D? gfxTicketFull = null;
        private Texture2D? gfxTicketHolders = null;
        private Texture2D? gfxTicketHolders2 = null;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            // space bar -> add a ticket for testing.
            if(handleInput && Input.KeyboardState.IsKeyDown(Keys.Space) && Input.LastKeyboardState.IsKeyUp(Keys.Space))
            {
                tickets.Add(new TicketObject(new Ticket() { customerId = 0 }));
                tickets[tickets.Count - 1].Initialize();
            }
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicketFull == null
                || gfxTicketHolders == null
                || gfxTicketHolders2 == null)
                return;

            for (int i = 0; i < tickets.Count; i++)
            {
                tickets[i].DrawLayer1(gameTime, spriteBatch);
            }
            spriteBatch.Draw(gfxTicketHolders, GlobalGraphics.Scale(new Rectangle(32, 0, gfxTicketHolders.Bounds.Width, gfxTicketHolders.Bounds.Height)), ThemeManager.GetColor("OrderStationTicketHolders"));
            for (int i = 0; i < tickets.Count; i++)
            {
                tickets[i].DrawLayer2(gameTime, spriteBatch);
            }
            //spriteBatch.Draw(gfxTicketFull, GlobalGraphics.Scale(new Rectangle(240, 4, gfxTicketFull.Bounds.Width, gfxTicketFull.Bounds.Height)), ThemeManager.GetColor("OrderStationTicket"));

            spriteBatch.Draw(gfxTicketHolders2, GlobalGraphics.Scale(new Rectangle(32, 0, gfxTicketHolders2.Bounds.Width, gfxTicketHolders2.Bounds.Height)), ThemeManager.GetColor("OrderStationTicketHolders2"));
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            TicketObject.LoadContent(contentManager, graphicsDevice);
            gfxTicketFull = GlobalContent.AddTexture("TicketFull", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketfull"));
            gfxTicketHolders = GlobalContent.AddTexture("TicketHolders", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketholders"));
            gfxTicketHolders2 = GlobalContent.AddTexture("TicketHolders2", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketholders2"));
        }
    }
}