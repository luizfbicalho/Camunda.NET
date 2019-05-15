using System;
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
namespace org.camunda.bpm.container.impl.jboss.test
{

	using BpmPlatformExtension = org.camunda.bpm.container.impl.jboss.extension.BpmPlatformExtension;
	using ModelConstants = org.camunda.bpm.container.impl.jboss.extension.ModelConstants;
	using AbstractSubsystemBaseTest = org.jboss.@as.subsystem.test.AbstractSubsystemBaseTest;
	using AfterClass = org.junit.AfterClass;
	using BeforeClass = org.junit.BeforeClass;

	/// 
	/// <summary>
	/// @author Tobias Metzke
	/// </summary>
	public class BpmPlatformSubsystemWithExpressionsTest : AbstractSubsystemBaseTest
	{

	  private static IDictionary<string, string> PROPERTIES = new Dictionary<string, string>();

	  static BpmPlatformSubsystemWithExpressionsTest()
	  {
		PROPERTIES["org.camunda.bpm.jboss.process-engine.test.isDefault"] = "true";
		PROPERTIES["org.camunda.bpm.jboss.job-executor.core-threads"] = "5";
		PROPERTIES["org.camunda.bpm.jboss.job-executor.max-threads"] = "15";
		PROPERTIES["org.camunda.bpm.jboss.job-executor.queue-length"] = "15";
		PROPERTIES["org.camunda.bpm.jboss.job-executor.keepalive-time"] = "10";
		PROPERTIES["org.camunda.bpm.jboss.job-executor.allow-core-timeout"] = "false";
	  }

	  public BpmPlatformSubsystemWithExpressionsTest() : base(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, new BpmPlatformExtension())
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void setUp()
	  public static void setUp()
	  {
		System.Properties.putAll(PROPERTIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDown()
	  public static void tearDown()
	  {
		foreach (string key in PROPERTIES.Keys)
		{
		  System.clearProperty(key);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected String getSubsystemXml() throws java.io.IOException
	  protected internal override string SubsystemXml
	  {
		  get
		  {
			try
			{
			  return FileUtils.readFile(JBossSubsystemXMLTest.SUBSYSTEM_WITH_ALL_OPTIONS_WITH_EXPRESSIONS);
			}
			catch (Exception e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
    
			return null;
		  }
	  }

	}

}