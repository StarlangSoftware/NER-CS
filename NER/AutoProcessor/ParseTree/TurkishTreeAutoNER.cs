using System;
using System.Collections.Generic;
using System.Globalization;
using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using Dictionary.Dictionary;

namespace NER.AutoProcessor.ParseTree
{
    public class TurkishTreeAutoNER : TreeAutoNER
    {
        public TurkishTreeAutoNER(ViewLayerType secondLanguage) : base(ViewLayerType.TURKISH_WORD)
        {
        }

        /// <summary>
        /// The method assigns the words "bay" and "bayan" PERSON tag. The method also checks the PERSON gazetteer, and if
        /// the word exists in the gazetteer, it assigns PERSON tag. The parent node should have the proper noun tag NNP.
        /// </summary>
        /// <param name="parseTree">The tree for which PERSON named entities checked.</param>
        protected override void AutoDetectPerson(ParseTreeDrawable parseTree)
        {
            NodeDrawableCollector nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            List<ParseNodeDrawable> leafList = nodeDrawableCollector.Collect();
            foreach (var parseNode in leafList){
                if (!parseNode.LayerExists(ViewLayerType.NER)){
                    String word = parseNode.GetLayerData(ViewLayerType.TURKISH_WORD).ToLower(new CultureInfo("tr"));
                    if (Word.IsHonorific(word) && parseNode.GetParent().GetData().GetName().Equals("NNP")){
                        parseNode.GetLayerInfo().SetLayerData(ViewLayerType.NER, "PERSON");
                    }
                    parseNode.CheckGazetteer(personGazetteer, word);
                }
            }
        }

        /// <summary>
        /// The method checks the LOCATION gazetteer, and if the word exists in the gazetteer, it assigns the LOCATION tag.
        /// </summary>
        /// <param name="parseTree">The tree for which LOCATION named entities checked.</param>
        protected override void AutoDetectLocation(ParseTreeDrawable parseTree)
        {
            NodeDrawableCollector nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            List<ParseNodeDrawable> leafList = nodeDrawableCollector.Collect();
            foreach (var parseNode in leafList){
                if (!parseNode.LayerExists(ViewLayerType.NER)){
                    String word = parseNode.GetLayerData(ViewLayerType.TURKISH_WORD).ToLower(new CultureInfo("tr"));
                    parseNode.CheckGazetteer(locationGazetteer, word);
                }
            }
        }

        /// <summary>
        /// The method assigns the words "corp.", "inc.", and "co" ORGANIZATION tag. The method also checks the
        /// ORGANIZATION gazetteer, and if the word exists in the gazetteer, it assigns ORGANIZATION tag.
        /// </summary>
        /// <param name="parseTree">The tree for which ORGANIZATION named entities checked.</param>
        protected override void AutoDetectOrganization(ParseTreeDrawable parseTree)
        {
            var nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            foreach (var parseNode in leafList){
                if (!parseNode.LayerExists(ViewLayerType.NER)){
                    String word = parseNode.GetLayerData(ViewLayerType.TURKISH_WORD).ToLower(new CultureInfo("tr"));
                    if (Word.IsOrganization(word)){
                        parseNode.GetLayerInfo().SetLayerData(ViewLayerType.NER, "ORGANIZATION");
                    }
                    parseNode.CheckGazetteer(organizationGazetteer, word);
                }
            }
        }

        /// <summary>
        /// The method checks for the MONEY entities using regular expressions. After that, if the expression is a MONEY
        /// expression, it also assigns the previous nodes, which may included numbers or some monetarial texts, MONEY tag.
        /// </summary>
        /// <param name="parseTree">The tree for which MONEY named entities checked.</param>
        protected override void AutoDetectMoney(ParseTreeDrawable parseTree)
        {
            NodeDrawableCollector nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            List<ParseNodeDrawable> leafList = nodeDrawableCollector.Collect();
            for (int i = 0; i < leafList.Count; i++) {
                ParseNodeDrawable parseNode = leafList[i];
                if (!parseNode.LayerExists(ViewLayerType.NER)){
                    String word = parseNode.GetLayerData(ViewLayerType.TURKISH_WORD).ToLower(new CultureInfo("tr"));
                    if (Word.IsMoney(word)) {
                        parseNode.GetLayerInfo().SetLayerData(ViewLayerType.NER, "MONEY");
                        int j = i - 1;
                        while (j >= 0){
                            ParseNodeDrawable previous = leafList[j];
                            if (previous.GetParent().GetData().GetName().Equals("CD")){
                                previous.GetLayerInfo().SetLayerData(ViewLayerType.NER, "MONEY");
                            } else {
                                break;
                            }
                            j--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The method checks for the TIME entities using regular expressions. After that, if the expression is a TIME
        /// expression, it also assigns the previous texts, which are numbers, TIME tag.
        /// </summary>
        /// <param name="parseTree">The tree for which TIME named entities checked.</param>
        protected override void AutoDetectTime(ParseTreeDrawable parseTree)
        {
            NodeDrawableCollector nodeDrawableCollector = new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            List<ParseNodeDrawable> leafList = nodeDrawableCollector.Collect();
            for (int i = 0; i < leafList.Count; i++){
                ParseNodeDrawable parseNode = leafList[i];
                if (!parseNode.LayerExists(ViewLayerType.NER)){
                    String word = parseNode.GetLayerData(ViewLayerType.TURKISH_WORD).ToLower(new CultureInfo("tr"));
                    if (Word.IsTime(word)){
                        parseNode.GetLayerInfo().SetLayerData(ViewLayerType.NER, "TIME");
                        if (i > 0){
                            ParseNodeDrawable previous = leafList[i - 1];
                            if (previous.GetParent().GetData().GetName().Equals("CD")){
                                previous.GetLayerInfo().SetLayerData(ViewLayerType.NER, "TIME");
                            }
                        }
                    }
                }
            }
        }
    }
}