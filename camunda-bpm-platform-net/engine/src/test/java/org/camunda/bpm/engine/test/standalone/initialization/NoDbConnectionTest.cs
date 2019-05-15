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
namespace org.camunda.bpm.engine.test.standalone.initialization
{

	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class NoDbConnectionTest : PvmTestCase
	{

	  public virtual void testNoDbConnection()
	  {
		try
		{
		  ProcessEngineConfiguration.createProcessEngineConfigurationFromResource("org/camunda/bpm/engine/test/standalone/initialization/nodbconnection.camunda.cfg.xml").buildProcessEngine();
		  fail("expected exception");
		}
		catch (Exception e)
		{
		  assertTrue(containsSqlException(e));
		}
	  }

	  private bool containsSqlException(Exception e)
	  {
		if (e == null)
		{
		  return false;
		}
		if (e is SQLException)
		{
		  return true;
		}
		return containsSqlException(e.InnerException);
	  }
	}

}