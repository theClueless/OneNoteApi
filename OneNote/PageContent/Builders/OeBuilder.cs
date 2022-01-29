using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OneNoteApi.Common;

namespace OneNoteApi.PageContent.Builders
{
    public static class OeBuilder
    {
        public static OE Build()
        {
            return new OE(new XElement(PageElementTypes.Oe));
        }

        public static OE WithText(this OE oe, string text)
        {
            oe.RawXml.Add(new XElement(PageElementTypes.Text, text));
            return oe;
        }

        public static OE WithBullet(this OE oe, int bulletNumber) // TODO: how do we know the right bullet number?
        {
            // <one:List>
            // < one:Bullet bullet = "3"
            // />
            //</ one:List >
            oe.RawXml.Add(
                new XElement(PageElementTypes.List, new XElement(PageElementTypes.Bullet, new XAttribute(Bullet.BulletNumberAttributeName, bulletNumber)))
                );
            return oe;
        }

        public static OE WithTag(this OE oe, int tagIndex)
        {
            oe.RawXml.Add(new XElement(PageElementTypes.Tag, 
                                                                new XAttribute(Tag.IndexAttributeName, tagIndex), 
                                                                new XAttribute(Tag.CompletedAttributeName, false)));
            return oe;
        }
    }
}
