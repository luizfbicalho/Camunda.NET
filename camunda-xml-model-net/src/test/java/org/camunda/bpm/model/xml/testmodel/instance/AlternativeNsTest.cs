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
namespace org.camunda.bpm.model.xml.testmodel.instance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using ModelImpl = org.camunda.bpm.model.xml.impl.ModelImpl;
	using ModelInstanceImpl = org.camunda.bpm.model.xml.impl.ModelInstanceImpl;
	using AbstractModelParser = org.camunda.bpm.model.xml.impl.parser.AbstractModelParser;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;
	using Parameters = org.junit.runners.Parameterized.Parameters;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// @author Ronny Bräunlich
	/// </summary>
	public class AlternativeNsTest : TestModelTest
	{

	  private const string MECHANICAL_NS = "http://camunda.org/mechanical";

	  public AlternativeNsTest(string testName, ModelInstance testModelInstance, AbstractModelParser modelParser) : base(testName, testModelInstance, modelParser)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "Model {0}") public static java.util.Collection<Object[]> models()
	  public static ICollection<object[]> models()
	  {
		return Collections.singleton(parseModel(typeof(AlternativeNsTest)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		modelInstance = cloneModelInstance();
		ModelImpl modelImpl = (ModelImpl) modelInstance.Model;
		modelImpl.declareAlternativeNamespace(MECHANICAL_NS, TestModelConstants.NEWER_NAMESPACE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		ModelImpl modelImpl = (ModelImpl) modelInstance.Model;
		modelImpl.undeclareAlternativeNamespace(MECHANICAL_NS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getUniqueChildElementByNameNsForAlternativeNs()
	  public virtual void getUniqueChildElementByNameNsForAlternativeNs()
	  {
		ModelElementInstance hedwig = modelInstance.getModelElementById("hedwig");
		assertThat(hedwig, @is(notNullValue()));
		ModelElementInstance childElementByNameNs = hedwig.getUniqueChildElementByNameNs(TestModelConstants.NEWER_NAMESPACE, "wings");
		assertThat(childElementByNameNs, @is(notNullValue()));
		assertThat(childElementByNameNs.TextContent, @is("wusch"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getChildElementsByTypeForAlternativeNs()
	  public virtual void getChildElementsByTypeForAlternativeNs()
	  {
		ModelElementInstance birdo = modelInstance.getModelElementById("birdo");
		assertThat(birdo, @is(notNullValue()));
		ICollection<Wings> elements = birdo.getChildElementsByType(typeof(Wings));
		assertThat(elements.Count, @is(1));
		assertThat(elements.GetEnumerator().next().TextContent, @is("zisch"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getAttributeValueNsForAlternativeNs()
	  public virtual void getAttributeValueNsForAlternativeNs()
	  {
		Bird plucky = modelInstance.getModelElementById("plucky");
		assertThat(plucky, @is(notNullValue()));
		bool? extendedWings = plucky.canHazExtendedWings();
		assertThat(extendedWings, @is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyingAttributeWithOldNamespaceKeepsOldNamespace()
	  public virtual void modifyingAttributeWithOldNamespaceKeepsOldNamespace()
	  {
		Bird plucky = modelInstance.getModelElementById("plucky");
		assertThat(plucky, @is(notNullValue()));
		//validate old value
		bool? extendedWings = plucky.canHazExtendedWings();
		assertThat(extendedWings, @is(false));
		//change it
		plucky.CanHazExtendedWings = true;
		string attributeValueNs = plucky.getAttributeValueNs(MECHANICAL_NS, "canHazExtendedWings");
		assertThat(attributeValueNs, @is("true"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyingAttributeWithNewNamespaceKeepsNewNamespace()
	  public virtual void modifyingAttributeWithNewNamespaceKeepsNewNamespace()
	  {
		Bird bird = createBird(modelInstance, "waldo", Gender.Male);
		bird.CanHazExtendedWings = true;
		string attributeValueNs = bird.getAttributeValueNs(TestModelConstants.NEWER_NAMESPACE, "canHazExtendedWings");
		assertThat(attributeValueNs, @is("true"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyingElementWithOldNamespaceKeepsOldNamespace()
	  public virtual void modifyingElementWithOldNamespaceKeepsOldNamespace()
	  {
		Bird birdo = modelInstance.getModelElementById("birdo");
		assertThat(birdo, @is(notNullValue()));
		Wings wings = birdo.Wings;
		assertThat(wings, @is(notNullValue()));
		wings.TextContent = "kawusch";

		IList<DomElement> childElementsByNameNs = birdo.DomElement.getChildElementsByNameNs(MECHANICAL_NS, "wings");
		assertThat(childElementsByNameNs.Count, @is(1));
		assertThat(childElementsByNameNs[0].TextContent, @is("kawusch"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyingElementWithNewNamespaceKeepsNewNamespace()
	  public virtual void modifyingElementWithNewNamespaceKeepsNewNamespace()
	  {
		Bird bird = createBird(modelInstance, "waldo", Gender.Male);
		bird.Wings = modelInstance.newInstance(typeof(Wings));

		IList<DomElement> childElementsByNameNs = bird.DomElement.getChildElementsByNameNs(TestModelConstants.NEWER_NAMESPACE, "wings");
		assertThat(childElementsByNameNs.Count, @is(1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void useExistingNamespace()
	  public virtual void useExistingNamespace()
	  {
		assertThatThereIsNoNewerNamespaceUrl();

		Bird plucky = modelInstance.getModelElementById("plucky");
		plucky.setAttributeValueNs(MECHANICAL_NS, "canHazExtendedWings", "true");
		assertThatThereIsNoNewerNamespaceUrl();

		assertTrue(plucky.canHazExtendedWings());
		assertThatThereIsNoNewerNamespaceUrl();
	  }

	  protected internal virtual void assertThatThereIsNoNewerNamespaceUrl()
	  {
		Node rootElement = modelInstance.Document.DomSource.Node.FirstChild;
		NamedNodeMap attributes = rootElement.Attributes;
		for (int i = 0; i < attributes.Length; i++)
		{
		  Node item = attributes.item(i);
		  string nodeValue = item.NodeValue;
		  assertNotEquals("Found newer namespace url which shouldn't exist", TestModelConstants.NEWER_NAMESPACE, nodeValue);
		}
	  }


	}

}