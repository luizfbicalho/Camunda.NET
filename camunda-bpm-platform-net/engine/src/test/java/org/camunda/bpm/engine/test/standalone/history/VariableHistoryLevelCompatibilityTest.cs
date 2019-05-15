﻿/*
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
namespace org.camunda.bpm.engine.test.standalone.history
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;


	/// <summary>
	/// This test ensures that if a user selects
	/// <seealso cref="ProcessEngineConfiguration#HISTORY_VARIABLE"/>, the level is internally
	/// mapped to <seealso cref="ProcessEngineConfigurationImpl#HISTORYLEVEL_ACTIVITY"/>.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class VariableHistoryLevelCompatibilityTest : ResourceProcessEngineTestCase
	{

	  public VariableHistoryLevelCompatibilityTest() : base("org/camunda/bpm/engine/test/standalone/history/variablehistory.camunda.cfg.xml")
	  {
	  }

	  public virtual void testCompatibilty()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		assertEquals(ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY, historyLevel);
	  }

	}

}