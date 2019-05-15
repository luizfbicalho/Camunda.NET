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
namespace org.camunda.bpm.engine.test.bpmn.exclusive
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ExclusiveTaskReuseCacheTest : ExclusiveTaskTest
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();
		processEngineConfiguration.DbEntityCacheReuseEnabled = true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		processEngineConfiguration.DbEntityCacheReuseEnabled = false;
		base.tearDown();
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/exclusive/ExclusiveTaskTest.testNonExclusiveService.bpmn20.xml"})]
	  public override void testNonExclusiveService()
	  {
		base.testNonExclusiveService();
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/exclusive/ExclusiveTaskTest.testExclusiveService.bpmn20.xml"})]
	  public override void testExclusiveService()
	  {
		base.testExclusiveService();
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/exclusive/ExclusiveTaskTest.testExclusiveServiceConcurrent.bpmn20.xml"})]
	  public override void testExclusiveServiceConcurrent()
	  {
		base.testExclusiveServiceConcurrent();
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/exclusive/ExclusiveTaskTest.testExclusiveSequence2.bpmn20.xml"})]
	  public override void testExclusiveSequence2()
	  {
		base.testExclusiveSequence2();
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/bpmn/exclusive/ExclusiveTaskTest.testExclusiveSequence3.bpmn20.xml"})]
	  public override void testExclusiveSequence3()
	  {
		base.testExclusiveSequence3();
	  }

	}

}