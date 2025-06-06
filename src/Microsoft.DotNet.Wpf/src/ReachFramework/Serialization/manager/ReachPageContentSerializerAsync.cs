// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*++                                                                         
    Abstract:
        This file contains the definition of a class that defines
        the common functionality required to serialize a PageContent.                                                                    
--*/
using System.Printing;
using System.Windows.Documents;

namespace System.Windows.Xps.Serialization
{
    /// <summary>
    /// Class defining common functionality required to
    /// serialize a ReachPageContentSerializer.
    /// </summary>
    internal class ReachPageContentSerializerAsync :
                   ReachSerializerAsync
    {
        #region Constructor

        /// <summary>
        /// Constructor for class ReachPageContentSerializer
        /// </summary>
        /// <param name="manager">
        /// The serialization manager, the services of which are
        /// used later in the serialization process of the type.
        /// </param>
        public
        ReachPageContentSerializerAsync(
            PackageSerializationManager   manager
            ):
        base(manager)
        {
            
        }

        #endregion Constructor
        
        #region Public Methods
        
        /// <summary>
        ///
        /// </summary>
        public
        override
        void
        AsyncOperation(
            ReachSerializerContext context
            )
        {
            if(context == null)
            {

            }
           
            switch (context.Action) 
            {
                case SerializerAction.serializePage:
                {
                    SerializePage(context.ObjectContext);
                    break;
                }
                
                default:
                {
                    base.AsyncOperation(context);
                    break;
                }
            }
        }
        

        #endregion

        
        #region Internal Methods

        /// <summary>
        /// The method is called once the object data is discovered at that 
        /// point of the serialization process.
        /// </summary>
        /// <param name="serializableObjectContext">
        /// The context of the object to be serialized at this time.
        /// </param>
        internal
        override
        void
        PersistObjectData(
            SerializableObjectContext   serializableObjectContext
            )
        {
            if(serializableObjectContext.IsComplexValue)
            {
                ReachSerializerContext context = new ReachSerializerContext(this,
                                                                            serializableObjectContext,
                                                                            SerializerAction.serializePage);

                ((IXpsSerializationManagerAsync)SerializationManager).OperationStack.Push(context);
            }
            else
            {
                throw new XpsSerializationException(SR.ReachSerialization_WrongPropertyTypeForPageContent);
            }
        }

        private
        void
        SerializePage(
            SerializableObjectContext   serializableObjectContext
            )
        {
            FixedPage fixedPage = Toolbox.GetPageRoot(serializableObjectContext.TargetObject);

            if(fixedPage != null)
            {
                ReachSerializer serializer = SerializationManager.GetSerializer(fixedPage);

                if(serializer!=null)
                {
                    XpsSerializationPrintTicketRequiredEventArgs e = 
                        new XpsSerializationPrintTicketRequiredEventArgs(PrintTicketLevel.FixedPagePrintTicket,
                                                         0);
                    ((IXpsSerializationManager)SerializationManager).OnXPSSerializationPrintTicketRequired(e);

                    PrintTicket printTicket = null;
                    if( e.Modified )
                    {
                        printTicket =  e.PrintTicket;
                    }
                    Toolbox.Layout(fixedPage, printTicket);

                    ((IXpsSerializationManager)SerializationManager).FixedPagePrintTicket = printTicket;

                    serializer.SerializeObject(fixedPage);
                }
                else
                {
                    throw new XpsSerializationException(SR.ReachSerialization_NoSerializer);
                }
            }

        }

        #endregion Internal Methods
    };
}
