using System;

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


	/// 
	/// <summary>
	/// @author christian.lipphardt@camunda.com
	/// </summary>
	public class BpmPlatformSubsystemEnginesOnlyTest : AbstractSubsystemBaseTest
	{

	  public BpmPlatformSubsystemEnginesOnlyTest() : base(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, new BpmPlatformExtension())
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected String getSubsystemXml() throws java.io.IOException
	  protected internal override string SubsystemXml
	  {
		  get
		  {
			try
			{
			  return FileUtils.readFile(JBossSubsystemXMLTest.SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS);
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