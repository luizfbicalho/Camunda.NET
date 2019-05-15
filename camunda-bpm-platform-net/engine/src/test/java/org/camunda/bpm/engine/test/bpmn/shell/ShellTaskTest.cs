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
namespace org.camunda.bpm.engine.test.bpmn.shell
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Test = org.junit.Test;

	public class ShellTaskTest : PluggableProcessEngineTestCase
	{

	  internal enum OsType
	  {
		LINUX,
		WINDOWS,
		MAC,
		SOLARIS,
		UNKOWN
	  }

	  internal OsType osType;

	  internal virtual OsType SystemOsType
	  {
		  get
		  {
			string osName = System.getProperty("os.name").ToLower();
			if (osName.IndexOf("win", StringComparison.Ordinal) >= 0)
			{
			  return OsType.WINDOWS;
			}
			else if (osName.IndexOf("mac", StringComparison.Ordinal) >= 0)
			{
			  return OsType.MAC;
			}
			else if ((osName.IndexOf("nix", StringComparison.Ordinal) >= 0) || (osName.IndexOf("nux", StringComparison.Ordinal) >= 0))
			{
			  return OsType.LINUX;
			}
			else if (osName.IndexOf("sunos", StringComparison.Ordinal) >= 0)
			{
			  return OsType.SOLARIS;
			}
			else
			{
			  return OsType.UNKOWN;
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		osType = SystemOsType;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOsDetection() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testOsDetection()
	  {
		assertTrue(osType != OsType.UNKOWN);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEchoShellWindows()
	  public virtual void testEchoShellWindows()
	  {
		if (osType == OsType.WINDOWS)
		{

		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("echoShellWindows");

		  string st = (string) runtimeService.getVariable(pi.Id, "resultVar");
		  assertNotNull(st);
		  assertTrue(st.StartsWith("EchoTest", StringComparison.Ordinal));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEchoShellLinux()
	  public virtual void testEchoShellLinux()
	  {
		if (osType == OsType.LINUX)
		{

		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("echoShellLinux");

		  string st = (string) runtimeService.getVariable(pi.Id, "resultVar");
		  assertNotNull(st);
		  assertTrue(st.StartsWith("EchoTest", StringComparison.Ordinal));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEchoShellMac()
	  public virtual void testEchoShellMac()
	  {
		if (osType == OsType.MAC)
		{

		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("echoShellMac");

		  string st = (string) runtimeService.getVariable(pi.Id, "resultVar");
		  assertNotNull(st);
		  assertTrue(st.StartsWith("EchoTest", StringComparison.Ordinal));
		}
	  }
	}

}