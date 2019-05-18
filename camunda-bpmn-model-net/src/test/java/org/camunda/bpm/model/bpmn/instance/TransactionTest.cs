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
namespace org.camunda.bpm.model.bpmn.instance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;



	using ReflectUtil = org.camunda.bpm.model.xml.impl.util.ReflectUtil;
	using Test = org.junit.Test;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class TransactionTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(SubProcess), false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("method", false, false, TransactionMethod.Compensate)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReadTransaction()
	  public virtual void shouldReadTransaction()
	  {
		Stream inputStream = ReflectUtil.getResourceAsStream("org/camunda/bpm/model/bpmn/TransactionTest.xml");
		Transaction transaction = Bpmn.readModelFromStream(inputStream).getModelElementById("transaction");

		assertThat(transaction).NotNull;
		assertThat(transaction.Method).isEqualTo(TransactionMethod.Image);
		assertThat(transaction.FlowElements).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldWriteTransaction() throws javax.xml.parsers.ParserConfigurationException, org.xml.sax.SAXException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldWriteTransaction()
	  {
		// given a model
		BpmnModelInstance newModel = Bpmn.createProcess("process").done();

		Process process = newModel.getModelElementById("process");

		Transaction transaction = newModel.newInstance(typeof(Transaction));
		transaction.Id = "transaction";
		transaction.Method = TransactionMethod.Store;
		process.addChildElement(transaction);

		// that is written to a stream
		MemoryStream outStream = new MemoryStream();
		Bpmn.writeModelToStream(outStream, newModel);

		// when reading from that stream
		MemoryStream inStream = new MemoryStream(outStream.toByteArray());

		DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
		DocumentBuilder docBuilder = docBuilderFactory.newDocumentBuilder();
		Document actualDocument = docBuilder.parse(inStream);

		// then it possible to traverse to the transaction element and assert its attributes
		NodeList transactionElements = actualDocument.getElementsByTagName("transaction");
		assertThat(transactionElements.Length).isEqualTo(1);

		Node transactionElement = transactionElements.item(0);
		assertThat(transactionElement).NotNull;
		Node methodAttribute = transactionElement.Attributes.getNamedItem("method");
		assertThat(methodAttribute.NodeValue).isEqualTo("##Store");

	  }
	}

}