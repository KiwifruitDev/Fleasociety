using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Fleasociety
{
    public class OrderStation : IStation {
        public string title { get; set; } = "Stations";
        public bool attention { get; set; } = false;
        private Texture2D? gfxTicket1 = null;
        private Texture2D? gfxTicket2 = null;
        private Texture2D? gfxTicketFull = null;
        private Texture2D? gfxTicketHolders = null;
        private Texture2D? gfxTicketHolders2 = null;
        public bool Update(GameTime gameTime, bool handleInput)
        {
            return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gfxTicket1 == null
                || gfxTicket2 == null
                || gfxTicketFull == null
                || gfxTicketHolders == null
                || gfxTicketHolders2 == null)
                return;
            Point ticketSize = gfxTicket1.Bounds.Size;
            spriteBatch.Draw(gfxTicket2, GlobalGraphics.Scale(new Rectangle(43 + ticketSize.X, 4, gfxTicket2.Bounds.Width, gfxTicket2.Bounds.Height)), ThemeManager.GetColor("OrderStationTicket"));
            spriteBatch.Draw(gfxTicketHolders, GlobalGraphics.Scale(new Rectangle(32, 0, gfxTicketHolders.Bounds.Width, gfxTicketHolders.Bounds.Height)), ThemeManager.GetColor("OrderStationTicketHolders"));
            spriteBatch.Draw(gfxTicket1, GlobalGraphics.Scale(new Rectangle(43, 4, gfxTicket1.Bounds.Width, gfxTicket1.Bounds.Height)), ThemeManager.GetColor("OrderStationTicket"));
            spriteBatch.Draw(gfxTicketFull, GlobalGraphics.Scale(new Rectangle(240, 4, gfxTicketFull.Bounds.Width, gfxTicketFull.Bounds.Height)), ThemeManager.GetColor("OrderStationTicket"));
            spriteBatch.Draw(gfxTicketHolders2, GlobalGraphics.Scale(new Rectangle(32, 0, gfxTicketHolders2.Bounds.Width, gfxTicketHolders2.Bounds.Height)), ThemeManager.GetColor("OrderStationTicketHolders2"));
        }
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            gfxTicket1 = GlobalContent.AddTexture("Ticket1", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket1"));
            gfxTicket2 = GlobalContent.AddTexture("Ticket2", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticket2"));
            gfxTicketFull = GlobalContent.AddTexture("TicketFull", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketfull"));
            gfxTicketHolders = GlobalContent.AddTexture("TicketHolders", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketholders"));
            gfxTicketHolders2 = GlobalContent.AddTexture("TicketHolders2", ThemeManager.LoadLayeredContent<Texture2D>("graphics/ticketholders2"));
        }
    }
}