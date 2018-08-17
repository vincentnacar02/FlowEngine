using FlowEngine.Core.activity;
using FlowEngine.Core.container;
using FlowEngine.Core.control_flow;
using FlowEngine.Core.elements.interfaces;
using FlowEngine.Core.elements.types;
using FlowEngine.Core.input;
using FlowEngine.Core.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core
{
    /// <summary>
    /// ElementFactory
    /// @author: Vincent Nacar
    /// </summary>
    public class ElementFactory
    {
        public static string DO_NODE = "Do/*";
        public static string ELSE_NODE = "Else/*";

        public static IElement CreateElement(XmlNode line)
        {
            IElement _element = null;
            switch (line.Name)
            {
                case "Activity":
                    _element = ElementFactory.CreateActivityElement(line);
                    break;
                case "If":
                    _element = ElementFactory.CreateControlFlowElement(line, elements.types.ControlFlowType.If);
                    break;
                case "ForEach":
                    _element = ElementFactory.CreateControlFlowElement(line, elements.types.ControlFlowType.ForEach);
                    break;
                case "Assign":
                    _element = ElementFactory.CreateAssignElemet(line);
                    break;
                case "Variable":
                    _element = ElementFactory.CreateVariableElement(line);
                    break;
                case "Logger":
                    _element = ElementFactory.CreateLoggerElement(line);
                    break;
                case "Repeat":
                    _element = ElementFactory.CreateControlFlowElement(line, elements.types.ControlFlowType.Repeat);
                    break;
                default:
                    break;
            }
            if (_element != null)
            {
                _element.InitializeElement();
                _element.ValidateRequiredAttributes();
            }
            return _element;
        }

        public static IElement CreateActivityElement(XmlNode node)
        {
            return new ActivityElement(node);
        }

        public static IElement CreateVariableElement(XmlNode node)
        {
            return new VariableElement(node);
        }

        public static IElement CreateControlFlowElement(XmlNode node, ControlFlowType type)
        {
            IElement _return = null;
            switch (type)
            {
                case ControlFlowType.If:
                    _return = new IfElement(node, CreateElements(node.SelectNodes(DO_NODE)), CreateElements(node.SelectNodes(ELSE_NODE)));
                    break;
                case ControlFlowType.ForEach:
                    _return = new ForEachElement(node, CreateElements(node.SelectNodes(DO_NODE)));
                    break;
                case ControlFlowType.Repeat:
                    _return = new RepeatElement(node, CreateElements(node.SelectNodes(DO_NODE)));
                    break;
                case ControlFlowType.While:
                    break;
                case ControlFlowType.Switch:
                    break;
                default:
                    break;
            }
            return _return;
        }

        public static IElement CreateAssignElemet(XmlNode node)
        {
            return new AssignElement(node);
        }

        public static IElement CreateLoggerElement(XmlNode node)
        {
            return new LoggerElement(node);
        }

        public static IList<IElement> CreateElements(XmlNodeList nodeList)
        {
            IList<IElement> _elements = new List<IElement>();
            foreach (XmlNode line in nodeList)
            {
                IElement lineElement = ElementFactory.CreateElement(line);
                _elements.Add(lineElement);
            }
            return _elements;
        }
    }
}
