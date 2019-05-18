using System.Collections.Generic;

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
namespace org.camunda.bpm.model.xml.instance
{
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using Gender = org.camunda.bpm.model.xml.testmodel.Gender;
	using TestModelParser = org.camunda.bpm.model.xml.testmodel.TestModelParser;
	using TestModelTest = org.camunda.bpm.model.xml.testmodel.TestModelTest;
	using Animals = org.camunda.bpm.model.xml.testmodel.instance.Animals;
	using Description = org.camunda.bpm.model.xml.testmodel.instance.Description;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.testmodel.TestModelConstants.MODEL_NAMESPACE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DomTest : TestModelTest
	{

	  private const string TEST_NS = "http://camunda.org/test";
	  private const string UNKNOWN_NS = "http://camunda.org/unknown";
	  private const string CAMUNDA_NS = "http://activiti.org/bpmn";
	  private const string FOX_NS = "http://www.camunda.com/fox";
	  private const string BPMN_NS = "http://www.omg.org/spec/BPMN/20100524/MODEL";

	  private DomDocument document;

	  public DomTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name="Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		return Arrays.asList(createModel(), parseModel(typeof(DomTest)));
	  }

	  private static object[] createModel()
	  {
		TestModelParser modelParser = new TestModelParser();
		ModelInstance modelInstance = modelParser.EmptyModel;

		Animals animals = modelInstance.newInstance(typeof(Animals));
		modelInstance.DocumentElement = animals;

		Description description = modelInstance.newInstance(typeof(Description));
		description.DomElement.addCDataSection("CDATA <test>");
		animals.addChildElement(description);

		return new object[]{"created", modelInstance, modelParser};
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void copyModelInstance()
	  public virtual void copyModelInstance()
	  {
		modelInstance = cloneModelInstance();
		document = modelInstance.Document;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRegisterNamespaces()
	  public virtual void testRegisterNamespaces()
	  {
		document.registerNamespace("test", TEST_NS);
		string prefix = document.registerNamespace(UNKNOWN_NS);
		assertThat(prefix).isEqualTo("ns0");

		DomElement rootElement = document.RootElement;
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "test")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "test")).isEqualTo(TEST_NS);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).isEqualTo(UNKNOWN_NS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGenerateNamespacePrefixes()
	  public virtual void testGenerateNamespacePrefixes()
	  {
		// occupy ns0 and ns2
		document.registerNamespace("ns0", UNKNOWN_NS + 0);
		document.registerNamespace("ns2", UNKNOWN_NS + 2);

		// add two generate
		string prefix = document.registerNamespace(UNKNOWN_NS + 1);
		assertThat(prefix).isEqualTo("ns1");
		prefix = document.registerNamespace(UNKNOWN_NS + 3);
		assertThat(prefix).isEqualTo("ns3");

		DomElement rootElement = document.RootElement;
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).isEqualTo(UNKNOWN_NS + 0);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns1")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns1")).isEqualTo(UNKNOWN_NS + 1);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns2")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns2")).isEqualTo(UNKNOWN_NS + 2);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns3")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns3")).isEqualTo(UNKNOWN_NS + 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDuplicateNamespaces()
	  public virtual void testDuplicateNamespaces()
	  {
		document.registerNamespace("test", TEST_NS);
		string prefix = document.registerNamespace(TEST_NS);
		assertThat(prefix).isEqualTo("test");
		prefix = document.registerNamespace(UNKNOWN_NS);
		assertThat(prefix).isEqualTo("ns0");
		prefix = document.registerNamespace(UNKNOWN_NS);
		assertThat(prefix).isEqualTo("ns0");

		DomElement rootElement = document.RootElement;
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "test")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "test")).isEqualTo(TEST_NS);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).isEqualTo(UNKNOWN_NS);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns1")).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testKnownPrefix()
	  public virtual void testKnownPrefix()
	  {
		document.registerNamespace(CAMUNDA_NS);
		document.registerNamespace(FOX_NS);

		DomElement rootElement = document.RootElement;
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "camunda")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "camunda")).isEqualTo(CAMUNDA_NS);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "fox")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "fox")).isEqualTo(FOX_NS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAlreadyUsedPrefix()
	  public virtual void testAlreadyUsedPrefix()
	  {
		document.registerNamespace("camunda", TEST_NS);
		document.registerNamespace(CAMUNDA_NS);

		DomElement rootElement = document.RootElement;
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "camunda")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "camunda")).isEqualTo(TEST_NS);
		assertThat(rootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).True;
		assertThat(rootElement.getAttribute(XMLNS_ATTRIBUTE_NS_URI, "ns0")).isEqualTo(CAMUNDA_NS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddElements()
	  public virtual void testAddElements()
	  {
		DomElement element = document.createElement(MODEL_NAMESPACE, "bird");
		element.setAttribute(MODEL_NAMESPACE, "gender", Gender.Unknown.ToString());
		document.RootElement.appendChild(element);
		assertThat(element.NamespaceURI).isEqualTo(MODEL_NAMESPACE);
		assertThat(element.LocalName).isEqualTo("bird");
		assertThat(element.Prefix).Null;
		assertThat(element.Document).isEqualTo(document);
		assertThat(element.RootElement).isEqualTo(document.RootElement);

		document.registerNamespace("test", TEST_NS);
		element = document.createElement(TEST_NS, "dog");
		document.RootElement.appendChild(element);
		assertThat(element.NamespaceURI).isEqualTo(TEST_NS);
		assertThat(element.LocalName).isEqualTo("dog");
		assertThat(element.Prefix).isEqualTo("test");
		assertThat(element.Document).isEqualTo(document);
		assertThat(element.RootElement).isEqualTo(document.RootElement);

		element = document.createElement(UNKNOWN_NS, "cat");
		document.RootElement.appendChild(element);
		assertThat(element.NamespaceURI).isEqualTo(UNKNOWN_NS);
		assertThat(element.LocalName).isEqualTo("cat");
		assertThat(element.Prefix).isEqualTo("ns0");
		assertThat(element.Document).isEqualTo(document);
		assertThat(element.RootElement).isEqualTo(document.RootElement);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddAttributes()
	  public virtual void testAddAttributes()
	  {
		DomElement element = document.createElement(MODEL_NAMESPACE, "bird");
		element.setAttribute(MODEL_NAMESPACE, "gender", Gender.Unknown.ToString());
		document.RootElement.appendChild(element);
		element.setIdAttribute("id", "tweety");
		element.setAttribute(MODEL_NAMESPACE, "name", "Tweety");
		assertThat(element.getAttribute(MODEL_NAMESPACE, "id")).isEqualTo("tweety");
		assertThat(element.getAttribute("name")).isEqualTo("Tweety");

		document.registerNamespace("test", TEST_NS);
		element = document.createElement(TEST_NS, "dog");
		document.RootElement.appendChild(element);
		element.setIdAttribute("id", "snoopy");
		element.setAttribute(TEST_NS, "name", "Snoopy");
		assertThat(element.getAttribute(TEST_NS, "id")).isEqualTo("snoopy");
		assertThat(element.getAttribute("name")).isEqualTo("Snoopy");

		element = document.createElement(UNKNOWN_NS, "cat");
		document.RootElement.appendChild(element);
		element.setIdAttribute("id", "sylvester");
		element.setAttribute(UNKNOWN_NS, "name", "Sylvester");
		element.setAttribute(BPMN_NS, "id", "test");
		assertThat(element.getAttribute(UNKNOWN_NS, "id")).isEqualTo("sylvester");
		assertThat(element.getAttribute("name")).isEqualTo("Sylvester");
		assertThat(element.getAttribute(BPMN_NS, "id")).isEqualTo("test");
		assertThat(element.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "bpmn2")).False;
		assertThat(document.RootElement.hasAttribute(XMLNS_ATTRIBUTE_NS_URI, "bpmn2")).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCData()
	  public virtual void testCData()
	  {
		Animals animals = (Animals) modelInstance.DocumentElement;
		assertThat(animals.Description.TextContent).isEqualTo("CDATA <test>");
	  }

	}

}