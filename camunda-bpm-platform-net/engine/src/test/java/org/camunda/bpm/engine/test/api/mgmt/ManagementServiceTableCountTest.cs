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
namespace org.camunda.bpm.engine.test.api.mgmt
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// @author Saeid Mizaei
	/// @author Joram Barrez
	/// </summary>
	public class ManagementServiceTableCountTest : PluggableProcessEngineTestCase
	{

	  public virtual void testTableCount()
	  {
		IDictionary<string, long> tableCount = managementService.TableCount;

		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		//commenting out this assertion as there is no much sense to check the quantity of records, not the presence/absence of specific ones
		//when additional row was added within CAM-7539, the test started failing when testing old engine (7.6) with new database (7.7)
		//assertEquals(new Long(5), tableCount.get(tablePrefix + "ACT_GE_PROPERTY"));

		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_GE_BYTEARRAY"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_RE_DEPLOYMENT"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_RU_EXECUTION"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_ID_GROUP"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_ID_MEMBERSHIP"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_ID_USER"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_RE_PROCDEF"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_RU_TASK"]);
		assertEquals(new long?(0), tableCount[tablePrefix + "ACT_RU_IDENTITYLINK"]);
	  }

	}

}