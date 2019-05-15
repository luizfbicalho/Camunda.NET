using System;
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
namespace org.camunda.bpm.engine.test.api.repository.diagram
{
	using ProcessDiagramLayoutFactory = org.camunda.bpm.engine.impl.bpmn.diagram.ProcessDiagramLayoutFactory;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DiagramLayout = org.camunda.bpm.engine.repository.DiagramLayout;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public class ProcessDiagramParseTest
	{

	  private const string resourcePath = "src/test/resources/org/camunda/bpm/engine/test/api/repository/diagram/testXxeParsingIsDisabled";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  internal bool xxeProcessingValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		xxeProcessingValue = processEngineConfiguration.EnableXxeProcessing;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		processEngineConfiguration.EnableXxeProcessing = xxeProcessingValue;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testXxeParsingIsDisabled()
	  public virtual void testXxeParsingIsDisabled()
	  {
		processEngineConfiguration.EnableXxeProcessing = false;

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream bpmnXmlStream = new java.io.FileInputStream(resourcePath + ".bpmn20.xml");
		  Stream bpmnXmlStream = new FileStream(resourcePath + ".bpmn20.xml", FileMode.Open, FileAccess.Read);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream imageStream = new java.io.FileInputStream(resourcePath + ".png");
		  Stream imageStream = new FileStream(resourcePath + ".png", FileMode.Open, FileAccess.Read);

		  assertNotNull(bpmnXmlStream);

		  // when we run this in the ProcessEngine context
		  engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, bpmnXmlStream, imageStream));
		  fail("The test model contains a DOCTYPE declaration! The test should fail.");
		}
		catch (FileNotFoundException ex)
		{
		  fail("The test BPMN model file is missing. " + ex.Message);
		}
		catch (Exception e)
		{
		  // then
		  assertThat(e.Message, containsString("Error while parsing BPMN model"));
		  assertThat(e.InnerException.Message, containsString("http://apache.org/xml/features/disallow-doctype-decl"));
		}
	  }

	  private class CommandAnonymousInnerClass : Command<DiagramLayout>
	  {
		  private readonly ProcessDiagramParseTest outerInstance;

		  private Stream bpmnXmlStream;
		  private Stream imageStream;

		  public CommandAnonymousInnerClass(ProcessDiagramParseTest outerInstance, Stream bpmnXmlStream, Stream imageStream)
		  {
			  this.outerInstance = outerInstance;
			  this.bpmnXmlStream = bpmnXmlStream;
			  this.imageStream = imageStream;
		  }

		  public DiagramLayout execute(CommandContext commandContext)
		  {
			return (new ProcessDiagramLayoutFactory()).getProcessDiagramLayout(bpmnXmlStream, imageStream);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testXxeParsingIsEnabled()
	  public virtual void testXxeParsingIsEnabled()
	  {
		processEngineConfiguration.EnableXxeProcessing = true;

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream bpmnXmlStream = new java.io.FileInputStream(resourcePath + ".bpmn20.xml");
		  Stream bpmnXmlStream = new FileStream(resourcePath + ".bpmn20.xml", FileMode.Open, FileAccess.Read);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream imageStream = new java.io.FileInputStream(resourcePath + ".png");
		  Stream imageStream = new FileStream(resourcePath + ".png", FileMode.Open, FileAccess.Read);

		  assertNotNull(bpmnXmlStream);

		  // when we run this in the ProcessEngine context
		  engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, bpmnXmlStream, imageStream));
		  fail("The test model contains a DOCTYPE declaration! The test should fail.");
		}
		catch (FileNotFoundException ex)
		{
		  fail("The test BPMN model file is missing. " + ex.Message);
		}
		catch (Exception e)
		{
		  // then
		  assertThat(e.Message, containsString("Error while parsing BPMN model"));
		  assertThat(e.InnerException.Message, containsString("file.txt"));
		}
	  }

	  private class CommandAnonymousInnerClass2 : Command<DiagramLayout>
	  {
		  private readonly ProcessDiagramParseTest outerInstance;

		  private Stream bpmnXmlStream;
		  private Stream imageStream;

		  public CommandAnonymousInnerClass2(ProcessDiagramParseTest outerInstance, Stream bpmnXmlStream, Stream imageStream)
		  {
			  this.outerInstance = outerInstance;
			  this.bpmnXmlStream = bpmnXmlStream;
			  this.imageStream = imageStream;
		  }

		  public DiagramLayout execute(CommandContext commandContext)
		  {
			return (new ProcessDiagramLayoutFactory()).getProcessDiagramLayout(bpmnXmlStream, imageStream);
		  }
	  }
	}

}