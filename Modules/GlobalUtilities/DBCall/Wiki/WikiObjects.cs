using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Wiki
{
    public enum WikiOwnerType
    {
        NotSet,
        Node,
        User
    }
    
    public enum WikiStatuses
    {
        Pending,
        Accepted,
        CitationNeeded
    }

    public class WikiTitle
    {
        private Guid? _TitleID;
        private Guid? _OwnerID;
        private Guid? _CreatorUserID;
        private DateTime? _CreationDate;
        private int? _SequenceNumber;
        private string _Title;
        private string _Status;
        private string _OwnerType;
        private Guid? _LastModifierUserID;
        private DateTime? _LastModificationDate;
        private List<Paragraph> _Paragraphs;
        private int? _RemovedParagraphsCount;


        public WikiTitle()
        {
            _Paragraphs = new List<Paragraph>();
        }

        public Guid? TitleID
        {
            get { return _TitleID; }
            set { _TitleID = value; }
        }

        public Guid? OwnerID
        {
            get { return _OwnerID; }
            set { _OwnerID = value; }
        }

        public Guid? CreatorUserID
        {
            get { return _CreatorUserID; }
            set { _CreatorUserID = value; }
        }

        public DateTime? CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value; }
        }

        public int? SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string OwnerType
        {
            get { return _OwnerType; }
            set { _OwnerType = value; }
        }

        public Guid? LastModifierUserID
        {
            get { return _LastModifierUserID; }
            set { _LastModifierUserID = value; }
        }

        public DateTime? LastModificationDate
        {
            get { return _LastModificationDate; }
            set { _LastModificationDate = value; }
        }

        public List<Paragraph> Paragraphs
        {
            get { return _Paragraphs; }
            set { _Paragraphs = value; }
        }

        public int? RemovedParagraphsCount
        {
            get { return _RemovedParagraphsCount; }
            set { _RemovedParagraphsCount = value; }
        }
    }

    public class Paragraph
    {
        private Guid? _ParagraphID;
        private Guid? _TitleID;
        private Guid? _CreatorUserID;
        private DateTime? _CreationDate;
        private string _Title;
        private string _BodyText;
        private int? _SequenceNumber;
        private bool? _IsRichText;
        private string _Status;
        private Guid? _LastModifierUserID;
        private DateTime? _LastModificationDate;
        private List<Change> _Changes;
        private List<DocFileInfo> _AttachedFiles;
        private int? _AppliedChangesCount;


        public Paragraph()
        {
            _Changes = new List<Change>();
            _AttachedFiles = new List<DocFileInfo>();
        }

        public Guid? ParagraphID
        {
            get { return _ParagraphID; }
            set { _ParagraphID = value; }
        }

        public Guid? TitleID
        {
            get { return _TitleID; }
            set { _TitleID = value; }
        }

        public Guid? CreatorUserID
        {
            get { return _CreatorUserID; }
            set { _CreatorUserID = value; }
        }

        public DateTime? CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string BodyText
        {
            get { return _BodyText; }
            set { _BodyText = value; }
        }

        public int? SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }

        public bool? IsRichText
        {
            get { return _IsRichText; }
            set { _IsRichText = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public Guid? LastModifierUserID
        {
            get { return _LastModifierUserID; }
            set { _LastModifierUserID = value; }
        }

        public DateTime? LastModificationDate
        {
            get { return _LastModificationDate; }
            set { _LastModificationDate = value; }
        }

        public List<Change> Changes
        {
            get { return _Changes; }
            set { _Changes = value; }
        }

        public List<DocFileInfo> AttachedFiles
        {
            get { return _AttachedFiles; }
            set { _AttachedFiles = value; }
        }

        public int? AppliedChangesCount
        {
            get { return _AppliedChangesCount; }
            set { _AppliedChangesCount = value; }
        }
    }

    public class Change
    {
        private Guid? _ChangeID;
        private Guid? _ParagraphID;
        private User _Sender;
        private DateTime? _SendDate;
        private DateTime? _LastModificationDate;
        private string _Title;
        private string _BodyText;
        private string _Status;
        private bool? _Applied;
        private List<DocFileInfo> _AttachedFiles;
        

        public Change()
        {
            _Sender = new User();
            _AttachedFiles = new List<DocFileInfo>();
        }

        public Guid? ChangeID
        {
            get { return _ChangeID; }
            set { _ChangeID = value; }
        }

        public Guid? ParagraphID
        {
            get { return _ParagraphID; }
            set { _ParagraphID = value; }
        }

        public User Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }

        public DateTime? SendDate
        {
            get { return _SendDate; }
            set { _SendDate = value; }
        }

        public DateTime? LastModificationDate
        {
            get { return _LastModificationDate; }
            set { _LastModificationDate = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string BodyText
        {
            get { return _BodyText; }
            set { _BodyText = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public bool? Applied
        {
            get { return _Applied; }
            set { _Applied = value; }
        }

        public List<DocFileInfo> AttachedFiles
        {
            get { return _AttachedFiles; }
            set { _AttachedFiles = value; }
        }
    }
}
