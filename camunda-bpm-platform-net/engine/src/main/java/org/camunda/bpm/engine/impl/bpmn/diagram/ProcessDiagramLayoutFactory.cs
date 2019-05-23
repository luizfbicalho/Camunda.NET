using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.bpmn.diagram
{
	using BpmnParser = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParser;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DiagramElement = org.camunda.bpm.engine.repository.DiagramElement;
	using DiagramLayout = org.camunda.bpm.engine.repository.DiagramLayout;
	using DiagramNode = org.camunda.bpm.engine.repository.DiagramNode;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;


	/// <summary>
	/// Provides positions and dimensions of elements in a process diagram as
	/// provided by <seealso cref="RepositoryService.getProcessDiagram(string)"/>.
	/// 
	/// @author Falko Menge
	/// </summary>
	public class ProcessDiagramLayoutFactory
	{

	  private const int GREY_THRESHOLD = 175;

	  // Parser features and their values needed to disable XXE Parsing
	  private static readonly IDictionary<string, bool> XXE_FEATURES = new HashMapAnonymousInnerClass();

	  private class HashMapAnonymousInnerClass : Dictionary<string, bool>
	  {
		  public HashMapAnonymousInnerClass() : base(4)
		  {
		  }

	//	  {
	//	put("http://apache.org/xml/features/disallow-doctype-decl", true);
	//	put("http://xml.org/sax/features/external-general-entities", false);
	//	put("http://xml.org/sax/features/external-parameter-entities", false);
	//	put("http://apache.org/xml/features/nonvalidating/load-external-dtd", false);
	//  }
	  }

	  /// <summary>
	  /// Provides positions and dimensions of elements in a process diagram as
	  /// provided by <seealso cref="RepositoryService.getProcessDiagram(string)"/>.
	  /// 
	  /// Currently, it only supports BPMN 2.0 models.
	  /// </summary>
	  /// <param name="bpmnXmlStream">
	  ///          BPMN 2.0 XML file </param>
	  /// <param name="imageStream">
	  ///          BPMN 2.0 diagram in PNG format (JPEG and other formats supported
	  ///          by <seealso cref="ImageIO"/> may also work) </param>
	  /// <returns> Layout of the process diagram </returns>
	  /// <returns> null when parameter imageStream is null </returns>
	  public virtual DiagramLayout getProcessDiagramLayout(Stream bpmnXmlStream, Stream imageStream)
	  {
		Document bpmnModel = parseXml(bpmnXmlStream);
		return getBpmnProcessDiagramLayout(bpmnModel, imageStream);
	  }

	  /// <summary>
	  /// Provides positions and dimensions of elements in a BPMN process diagram as
	  /// provided by <seealso cref="RepositoryService.getProcessDiagram(string)"/>.
	  /// </summary>
	  /// <param name="bpmnModel">
	  ///          BPMN 2.0 XML document </param>
	  /// <param name="imageStream">
	  ///          BPMN 2.0 diagram in PNG format (JPEG and other formats supported
	  ///          by <seealso cref="ImageIO"/> may also work) </param>
	  /// <returns> Layout of the process diagram </returns>
	  /// <returns> null when parameter imageStream is null </returns>
	  public virtual DiagramLayout getBpmnProcessDiagramLayout(Document bpmnModel, Stream imageStream)
	  {
		if (imageStream == null)
		{
		  return null;
		}
		DiagramNode diagramBoundsXml = getDiagramBoundsFromBpmnDi(bpmnModel);
		DiagramNode diagramBoundsImage;
		if (isExportedFromAdonis50(bpmnModel))
		{
		  int offsetTop = 29; // Adonis header
		  int offsetBottom = 61; // Adonis footer
		  diagramBoundsImage = getDiagramBoundsFromImage(imageStream, offsetTop, offsetBottom);
		}
		else
		{
		  diagramBoundsImage = getDiagramBoundsFromImage(imageStream);
		}

		IDictionary<string, DiagramNode> listOfBounds = new Dictionary<string, DiagramNode>();
		listOfBounds[diagramBoundsXml.Id] = diagramBoundsXml;
	//    listOfBounds.putAll(getElementBoundsFromBpmnDi(bpmnModel));
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		listOfBounds.putAll(fixFlowNodePositionsIfModelFromAdonis(bpmnModel, getElementBoundsFromBpmnDi(bpmnModel)));

		IDictionary<string, DiagramElement> listOfBoundsForImage = transformBoundsForImage(diagramBoundsImage, diagramBoundsXml, listOfBounds);
		return new DiagramLayout(listOfBoundsForImage);
	  }

	  protected internal virtual Document parseXml(Stream bpmnXmlStream)
	  {
		// Initiate DocumentBuilderFactory
		DocumentBuilderFactory factory = ConfiguredDocumentBuilderFactory;
		DocumentBuilder builder;
		Document bpmnModel;
		try
		{
		  // Get DocumentBuilder
		  builder = factory.newDocumentBuilder();
		  // Parse and load the Document into memory
		  bpmnModel = builder.parse(bpmnXmlStream);
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Error while parsing BPMN model.", e);
		}
		return bpmnModel;
	  }

	  protected internal virtual DiagramNode getDiagramBoundsFromBpmnDi(Document bpmnModel)
	  {
		double? minX = null;
		double? minY = null;
		double? maxX = null;
		double? maxY = null;

		// Node positions and dimensions
		NodeList setOfBounds = bpmnModel.getElementsByTagNameNS(BpmnParser.BPMN_DC_NS, "Bounds");
		for (int i = 0; i < setOfBounds.Length; i++)
		{
		  Element element = (Element) setOfBounds.item(i);
		  double? x = Convert.ToDouble(element.getAttribute("x"));
		  double? y = Convert.ToDouble(element.getAttribute("y"));
		  double? width = Convert.ToDouble(element.getAttribute("width"));
		  double? height = Convert.ToDouble(element.getAttribute("height"));

		  if (x == 0.0 && y == 0.0 && width == 0.0 && height == 0.0)
		  {
			// Ignore empty labels like the ones produced by Yaoqiang:
			// <bpmndi:BPMNLabel><dc:Bounds height="0.0" width="0.0" x="0.0" y="0.0"/></bpmndi:BPMNLabel>
		  }
		  else
		  {
			if (minX == null || x < minX)
			{
			  minX = x;
			}
			if (minY == null || y < minY)
			{
			  minY = y;
			}
			if (maxX == null || maxX < (x + width))
			{
			  maxX = (x + width);
			}
			if (maxY == null || maxY < (y + height))
			{
			  maxY = (y + height);
			}
		  }
		}

		// Edge bend points
		NodeList waypoints = bpmnModel.getElementsByTagNameNS(BpmnParser.OMG_DI_NS, "waypoint");
		for (int i = 0; i < waypoints.Length; i++)
		{
		  Element waypoint = (Element) waypoints.item(i);
		  double? x = Convert.ToDouble(waypoint.getAttribute("x"));
		  double? y = Convert.ToDouble(waypoint.getAttribute("y"));

		  if (minX == null || x < minX)
		  {
			minX = x;
		  }
		  if (minY == null || y < minY)
		  {
			minY = y;
		  }
		  if (maxX == null || maxX < x)
		  {
			maxX = x;
		  }
		  if (maxY == null || maxY < y)
		  {
			maxY = y;
		  }
		}

		DiagramNode diagramBounds = new DiagramNode("BPMNDiagram");
		diagramBounds.X = minX;
		diagramBounds.Y = minY;
		diagramBounds.Width = maxX - minX;
		diagramBounds.Height = maxY - minY;
		return diagramBounds;
	  }

	  protected internal virtual DiagramNode getDiagramBoundsFromImage(Stream imageStream)
	  {
		return getDiagramBoundsFromImage(imageStream, 0, 0);
	  }

	  protected internal virtual DiagramNode getDiagramBoundsFromImage(Stream imageStream, int offsetTop, int offsetBottom)
	  {
		BufferedImage image;
		try
		{
		  image = ImageIO.read(imageStream);
		}
		catch (IOException e)
		{
		  throw new ProcessEngineException("Error while reading process diagram image.", e);
		}
		DiagramNode diagramBoundsImage = getDiagramBoundsFromImage(image, offsetTop, offsetBottom);
		return diagramBoundsImage;
	  }

	  protected internal virtual DiagramNode getDiagramBoundsFromImage(BufferedImage image, int offsetTop, int offsetBottom)
	  {
		int width = image.Width;
		int height = image.Height;

		IDictionary<int, bool> rowIsWhite = new SortedDictionary<int, bool>();
		IDictionary<int, bool> columnIsWhite = new SortedDictionary<int, bool>();

		for (int row = 0; row < height; row++)
		{
		  if (!rowIsWhite.ContainsKey(row))
		  {
			rowIsWhite[row] = true;
		  }
		  if (row <= offsetTop || row > image.Height - offsetBottom)
		  {
			rowIsWhite[row] = true;
		  }
		  else
		  {
			for (int column = 0; column < width; column++)
			{
			  if (!columnIsWhite.ContainsKey(column))
			  {
				columnIsWhite[column] = true;
			  }
			  int pixel = image.getRGB(column, row);
			  int alpha = (pixel >> 24) & 0xff;
			  int red = (pixel >> 16) & 0xff;
			  int green = (pixel >> 8) & 0xff;
			  int blue = (pixel >> 0) & 0xff;
			  if (!(alpha == 0 || (red >= GREY_THRESHOLD && green >= GREY_THRESHOLD && blue >= GREY_THRESHOLD)))
			  {
				rowIsWhite[row] = false;
				columnIsWhite[column] = false;
			  }
			}
		  }
		}

		int marginTop = 0;
		for (int row = 0; row < height; row++)
		{
		  if (rowIsWhite[row])
		  {
			++marginTop;
		  }
		  else
		  {
			// Margin Top Found
			break;
		  }
		}

		int marginLeft = 0;
		for (int column = 0; column < width; column++)
		{
		  if (columnIsWhite[column])
		  {
			++marginLeft;
		  }
		  else
		  {
			// Margin Left Found
			break;
		  }
		}

		int marginRight = 0;
		for (int column = width - 1; column >= 0; column--)
		{
		  if (columnIsWhite[column])
		  {
			++marginRight;
		  }
		  else
		  {
			// Margin Right Found
			break;
		  }
		}

		int marginBottom = 0;
		for (int row = height - 1; row >= 0; row--)
		{
		  if (rowIsWhite[row])
		  {
			++marginBottom;
		  }
		  else
		  {
			// Margin Bottom Found
			break;
		  }
		}

		DiagramNode diagramBoundsImage = new DiagramNode();
		diagramBoundsImage.X = (double) marginLeft;
		diagramBoundsImage.Y = (double) marginTop;
		diagramBoundsImage.Width = (double)(width - marginRight - marginLeft);
		diagramBoundsImage.Height = (double)(height - marginBottom - marginTop);
		return diagramBoundsImage;
	  }

	  protected internal virtual IDictionary<string, DiagramNode> getElementBoundsFromBpmnDi(Document bpmnModel)
	  {
		IDictionary<string, DiagramNode> listOfBounds = new Dictionary<string, DiagramNode>();
		// iterate over all DI shapes
		NodeList shapes = bpmnModel.getElementsByTagNameNS(BpmnParser.BPMN_DI_NS, "BPMNShape");
		for (int i = 0; i < shapes.Length; i++)
		{
		  Element shape = (Element) shapes.item(i);
		  string bpmnElementId = shape.getAttribute("bpmnElement");
		  // get bounds of shape
		  NodeList childNodes = shape.ChildNodes;
		  for (int j = 0; j < childNodes.Length; j++)
		  {
			Node childNode = childNodes.item(j);
			if (childNode is Element && BpmnParser.BPMN_DC_NS.Equals(childNode.NamespaceURI) && "Bounds".Equals(childNode.LocalName))
			{
			  DiagramNode bounds = parseBounds((Element) childNode);
			  bounds.Id = bpmnElementId;
			  listOfBounds[bpmnElementId] = bounds;
			  break;
			}
		  }
		}
		return listOfBounds;
	  }

	  protected internal virtual DiagramNode parseBounds(Element boundsElement)
	  {
		DiagramNode bounds = new DiagramNode();
		bounds.X = Convert.ToDouble(boundsElement.getAttribute("x"));
		bounds.Y = Convert.ToDouble(boundsElement.getAttribute("y"));
		bounds.Width = Convert.ToDouble(boundsElement.getAttribute("width"));
		bounds.Height = Convert.ToDouble(boundsElement.getAttribute("height"));
		return bounds;
	  }

	  protected internal virtual IDictionary<string, DiagramElement> transformBoundsForImage(DiagramNode diagramBoundsImage, DiagramNode diagramBoundsXml, IDictionary<string, DiagramNode> listOfBounds)
	  {
		IDictionary<string, DiagramElement> listOfBoundsForImage = new Dictionary<string, DiagramElement>();
		foreach (KeyValuePair<string, DiagramNode> bounds in listOfBounds.SetOfKeyValuePairs())
		{
		  listOfBoundsForImage[bounds.Key] = transformBoundsForImage(diagramBoundsImage, diagramBoundsXml, bounds.Value);
		}
		return listOfBoundsForImage;
	  }

	  protected internal virtual DiagramNode transformBoundsForImage(DiagramNode diagramBoundsImage, DiagramNode diagramBoundsXml, DiagramNode elementBounds)
	  {
		double scalingFactorX = diagramBoundsImage.Width / diagramBoundsXml.Width;
		double scalingFactorY = diagramBoundsImage.Width / diagramBoundsXml.Width;

		DiagramNode elementBoundsForImage = new DiagramNode(elementBounds.Id);
		elementBoundsForImage.X = (double) (long)Math.Round((elementBounds.X - diagramBoundsXml.X) * scalingFactorX + diagramBoundsImage.X, MidpointRounding.AwayFromZero);
		elementBoundsForImage.Y = (double) (long)Math.Round((elementBounds.Y - diagramBoundsXml.Y) * scalingFactorY + diagramBoundsImage.Y, MidpointRounding.AwayFromZero);
		elementBoundsForImage.Width = (double) (long)Math.Round(elementBounds.Width * scalingFactorX, MidpointRounding.AwayFromZero);
		elementBoundsForImage.Height = (double) (long)Math.Round(elementBounds.Height * scalingFactorY, MidpointRounding.AwayFromZero);
		return elementBoundsForImage;
	  }

	  protected internal virtual IDictionary<string, DiagramNode> fixFlowNodePositionsIfModelFromAdonis(Document bpmnModel, IDictionary<string, DiagramNode> elementBoundsFromBpmnDi)
	  {
		if (isExportedFromAdonis50(bpmnModel))
		{
		  IDictionary<string, DiagramNode> mapOfFixedBounds = new Dictionary<string, DiagramNode>();
		  XPathFactory xPathFactory = XPathFactory.newInstance();
		  XPath xPath = xPathFactory.newXPath();
		  xPath.NamespaceContext = new Bpmn20NamespaceContext();
		  foreach (KeyValuePair<string, DiagramNode> entry in elementBoundsFromBpmnDi.SetOfKeyValuePairs())
		  {
			string elementId = entry.Key;
			DiagramNode elementBounds = entry.Value;
			string expression = "local-name(//bpmn:*[@id = '" + elementId + "'])";
			try
			{
			  XPathExpression xPathExpression = xPath.compile(expression);
			  string elementLocalName = xPathExpression.evaluate(bpmnModel);
			  if (!"participant".Equals(elementLocalName) && !"lane".Equals(elementLocalName) && !"textAnnotation".Equals(elementLocalName) && !"group".Equals(elementLocalName))
			  {
				elementBounds.X = elementBounds.X - elementBounds.Width / 2;
				elementBounds.Y = elementBounds.Y - elementBounds.Height / 2;
			  }
			}
			catch (XPathExpressionException e)
			{
			  throw new ProcessEngineException("Error while evaluating the following XPath expression on a BPMN XML document: '" + expression + "'.", e);
			}
			mapOfFixedBounds[elementId] = elementBounds;
		  }
		  return mapOfFixedBounds;
		}
		else
		{
		  return elementBoundsFromBpmnDi;
		}
	  }

	  protected internal virtual bool isExportedFromAdonis50(Document bpmnModel)
	  {
		return "ADONIS".Equals(bpmnModel.DocumentElement.getAttribute("exporter")) && "5.0".Equals(bpmnModel.DocumentElement.getAttribute("exporterVersion"));
	  }

	  protected internal virtual DocumentBuilderFactory ConfiguredDocumentBuilderFactory
	  {
		  get
		  {
    
			bool isXxeParsingEnabled = Context.ProcessEngineConfiguration.EnableXxeProcessing;
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
    
			// Configure XXE Processing
			try
			{
			  foreach (KeyValuePair<string, bool> feature in XXE_FEATURES.entrySet())
			  {
				factory.setFeature(feature.Key, isXxeParsingEnabled ^ feature.Value);
			  }
			}
			catch (Exception e)
			{
			  throw new ProcessEngineException("Error while configuring BPMN parser.", e);
			}
			factory.XIncludeAware = isXxeParsingEnabled;
			factory.ExpandEntityReferences = isXxeParsingEnabled;
    
			// Get a factory that understands namespaces
			factory.NamespaceAware = true;
    
			return factory;
		  }
	  }
	}

}