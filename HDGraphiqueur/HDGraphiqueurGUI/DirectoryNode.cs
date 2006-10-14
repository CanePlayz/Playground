using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace HDGraph
{
    public class DirectoryNode : IXmlSerializable
    {
        #region Variables et propri�t�s 

        private long totalSize;
        /// <summary>
        /// Taille total en octet du r�pertoire
        /// </summary>
        public long TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }

        private long filesSize;
        /// <summary>
        /// Taille en octet de l'ensemble des fichiers du r�pertoire
        /// </summary>
        public long FilesSize
        {
            get { return filesSize; }
            set { filesSize = value; }
        }


        private string name;
        /// <summary>
        /// Nom du r�pertoire
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string path;
        /// <summary>
        /// Chemin du r�pertoire
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private DirectoryNode parent;
        /// <summary>
        /// R�petoire parent
        /// </summary>
        public DirectoryNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private List<DirectoryNode> children = new List<DirectoryNode>();
        /// <summary>
        /// Liste des r�pertoires contenus dans ce r�pertoire.
        /// </summary>
        public List<DirectoryNode> Children
        {
            get { return children; }
            set { children = value; }
        }

        private int profondeurMax = 1;
        /// <summary>
        /// Plus grande profondeur calcul�e sur le r�pertoire courant.
        /// </summary>
        public int ProfondeurMax
        {
            get { return profondeurMax; }
            set { profondeurMax = value; }
        }

        private bool existsUncalcSubdir;
        /// <summary>
        /// Booleen indiquant s'il existe des r�pertoires enfants qui n'ont pas �t� calcul�s.
        /// </summary>
        public bool ExistsUncalcSubDir
        {
            get { return existsUncalcSubdir; }
            set { existsUncalcSubdir = value; }
        }


        #endregion

        #region Constructeur(s)

        public DirectoryNode(string path)
        {
            this.path = path;
            this.name = GetNameFromPath(path);
        }

        internal DirectoryNode()
        {

        }

        #endregion

        #region M�thodes
        
        public override string ToString()
        {
            return base.ToString() + ": " + name;
        }

        private string GetNameFromPath(string path)
        {
            string theName = System.IO.Path.GetFileName(path);
            if (theName == null || theName.Length == 0)
                theName = path;
            return theName;
        }

        /// <summary>
        /// Se base sur le nom du node courant et sur le path du p�re pour mettre � jour le path courant.
        /// </summary>
        private void UpdatePathFromNameAndParent()
        {
            if (parent != null)
                this.path = parent.path + System.IO.Path.DirectorySeparatorChar + name;
            foreach (DirectoryNode node in children)
            {
                node.UpdatePathFromNameAndParent();
            }
        }

        #endregion

        #region IXmlSerializable Membres

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // D�but �l�ment DirectoryNode
            reader.ReadStartElement();

            name = reader.ReadElementContentAsString();
            totalSize = reader.ReadElementContentAsLong();
            filesSize = reader.ReadElementContentAsLong();
            profondeurMax = reader.ReadElementContentAsInt();

            // D�but �l�ment Children
            reader.ReadStartElement("Children");
            XmlSerializer serializer = new XmlSerializer(typeof(List<DirectoryNode>));
            children = (List<DirectoryNode>)serializer.Deserialize(reader);
            // Fin �l�ment Children
            reader.ReadEndElement();

            // Mise � jour du parent
            foreach (DirectoryNode node in children)
            {
                node.parent = this;
            }

            // Mise � jour du path
            if (name.Contains(":"))
            {
                path = name;
                name = GetNameFromPath(path);
                UpdatePathFromNameAndParent();
            }

            // Fin �l�ment DirectoryNode
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            //string ns = "http://HDGraphiqueur.tools.laugel.fr/DirectoryNode.xsd";
            if (parent == null)
                writer.WriteElementString("Name", path);
            else
                writer.WriteElementString("Name", name);
            writer.WriteElementString("TotalSize", totalSize.ToString());
            writer.WriteElementString("FilesSize", filesSize.ToString());
            writer.WriteElementString("ProfondeurMax", profondeurMax.ToString());

            writer.WriteStartElement("Children");
            XmlSerializer serializer = new XmlSerializer(typeof(List<DirectoryNode>));
            serializer.Serialize(writer, children);
            writer.WriteEndElement();

            // Pour s�rialiser g�n�riquement les propri�t�s d'un  objet :
            //foreach (PropertyInfo prop in this.GetType().GetProperties())
            //{
            //    if (prop.GetAccessors().Length > 1  // il faut un get et un set
            //        && prop.GetAccessors()[0].IsPublic   // chacun des 2 doivent �tre publiques
            //        && prop.GetAccessors()[1].IsPublic   // chacun des 2 doivent �tre publiques
            //        && prop.Name != "Parent"
            //        && prop.Name != "Path"
            //        && prop.Name != "Name"
            //        && prop.Name != "ProfondeurMax")
            //    {
            //        writer.WriteStartElement(prop.Name);
            //        XmlSerializer serializer = new XmlSerializer(prop.PropertyType);
            //        serializer.Serialize(writer, prop.GetValue(this, null));
            //        writer.WriteEndElement();
            //    }
            //}
        }

        #endregion
    }
}
