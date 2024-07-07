using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;

namespace NER.AutoProcessor.ParseTree
{
    /**
     * Abstract class to detect named entities in a tree automatically. By implementing 5 abstract methods,
     * the class can detect (i) Person, (ii) Location, (iii) Organization, (iv) Money, (v) Time.
     * Each method tries to detect those named entities and if successful, sets the correct named entity for the word.
     * Anything that is denoted by a proper name, i. e., for instance, a person, a location, or an organization,
     * is considered to be a named entity. In addition, named entities also include things like dates, times,
     * or money. Here is a sample text with named entities marked:
     * [$_{ORG}$ Türk Hava Yolları] bu [$_{TIME}$ Pazartesi'den] itibaren [$_{LOC}$ İstanbul] [$_{LOC}$ Ankara]
     * güzergahı için indirimli satışlarını [$_{MONEY}$ 90 TL'den] başlatacağını açıkladı.
     * This sentence contains 5 named entities including 3 words labeled as ORGANIZATION, 2 words labeled as
     * LOCATION, 1 word labeled as TIME, and 1 word labeled as MONEY.
     * */
    public abstract class TreeAutoNER : NamedEntityRecognition.AutoNER
    {
        protected ViewLayerType secondLanguage;

        /// <summary>
        /// The method should detect PERSON named entities. PERSON corresponds to people or
        /// characters. Example: {\bf Atatürk} yurdu düşmanlardan kurtardı.
        /// </summary>
        /// <param name="parseTree">The tree for which PERSON named entities checked.</param>
        protected abstract void AutoDetectPerson(ParseTreeDrawable parseTree);
        
        /// <summary>
        /// The method should detect LOCATION named entities. LOCATION corresponds to regions,
        /// mountains, seas. Example: Ülkemizin başkenti {\bf Ankara'dır}.
        /// </summary>
        /// <param name="parseTree">The tree for which LOCATION named entities checked.</param>
        protected abstract void AutoDetectLocation(ParseTreeDrawable parseTree);
        
        /// <summary>
        /// The method should detect ORGANIZATION named entities. ORGANIZATION corresponds to companies,
        /// teams etc. Example:  {\bf IMKB} günü 60 puan yükselerek kapattı.
        /// </summary>
        /// <param name="parseTree">The tree for which ORGANIZATION named entities checked.</param>
        protected abstract void AutoDetectOrganization(ParseTreeDrawable parseTree);
        
        /// <summary>
        /// The method should detect MONEY named entities. MONEY corresponds to monetarial
        /// expressions. Example: Geçen gün {\bf 3000 TL} kazandık.
        /// </summary>
        /// <param name="parseTree">The tree for which MONEY named entities checked.</param>
        protected abstract void AutoDetectMoney(ParseTreeDrawable parseTree);
        
        /// <summary>
        /// The method should detect TIME named entities. TIME corresponds to time
        /// expressions. Example: {\bf Cuma günü} tatil yapacağım.
        /// </summary>
        /// <param name="parseTree">The tree for which TIME named entities checked.</param>
        protected abstract void AutoDetectTime(ParseTreeDrawable parseTree);

        /// <summary>
        /// Constructor for the TreeAutoNER. Sets the language for the NER annotation. Currently, the system supports Turkish
        /// and Persian.
        /// </summary>
        /// <param name="secondLanguage">Language for NER annotation.</param>
        protected TreeAutoNER(ViewLayerType secondLanguage)
        {
            this.secondLanguage = secondLanguage;
        }

        /// <summary>
        /// The main method to automatically detect named entities in a tree. The algorithm
        /// 1. Detects PERSON(s).
        /// 2. Detects LOCATION(s).
        /// 3. Detects ORGANIZATION(s).
        /// 4. Detects MONEY.
        /// 5. Detects TIME.
        /// For not detected nodes, the algorithm sets the named entity "NONE".
        /// </summary>
        /// <param name="parseTree">The tree for which named entities checked.</param>
        public void AutoNer(ParseTreeDrawable parseTree)
        {
            AutoDetectPerson(parseTree);
            AutoDetectLocation(parseTree);
            AutoDetectOrganization(parseTree);
            AutoDetectMoney(parseTree);
            AutoDetectTime(parseTree);
            var nodeDrawableCollector =
                new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTransferable(secondLanguage));
            var leafList = nodeDrawableCollector.Collect();
            foreach (var parseNode in leafList){
                if (!parseNode.LayerExists(ViewLayerType.NER))
                {
                    parseNode.GetLayerInfo().SetLayerData(ViewLayerType.NER, "NONE");
                }
            }
            parseTree.Save();
        }
    }
}