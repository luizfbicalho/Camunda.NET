using System;
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
namespace org.camunda.bpm.engine.test.cmmn.transformer
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using DefaultCmmnElementHandlerRegistry = org.camunda.bpm.engine.impl.cmmn.handler.DefaultCmmnElementHandlerRegistry;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnTransform = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransform;
	using CmmnTransformer = org.camunda.bpm.engine.impl.cmmn.transformer.CmmnTransformer;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using Cmmn = org.camunda.bpm.model.cmmn.Cmmn;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CmmnModelElementInstance = org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using PlanItem = org.camunda.bpm.model.cmmn.instance.PlanItem;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using IoUtil = org.camunda.bpm.model.xml.impl.util.IoUtil;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnTransformerTest
	{

	  protected internal CmmnTransform transformer;
	  protected internal CmmnModelInstance modelInstance;
	  protected internal Definitions definitions;
	  protected internal Case caseDefinition;
	  protected internal CasePlanModel casePlanModel;
	  protected internal DeploymentEntity deployment;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		CmmnTransformer transformerWrapper = new CmmnTransformer(null, new DefaultCmmnElementHandlerRegistry(), null);
		transformer = new CmmnTransform(transformerWrapper);

		deployment = new DeploymentEntity();
		deployment.Id = "aDeploymentId";

		transformer.Deployment = deployment;

		modelInstance = Cmmn.createEmptyModel();
		definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = "http://camunda.org/examples";
		modelInstance.Definitions = definitions;

		caseDefinition = createElement(definitions, "aCaseDefinition", typeof(Case));
		casePlanModel = createElement(caseDefinition, "aCasePlanModel", typeof(CasePlanModel));
	  }

	  protected internal virtual T createElement<T>(CmmnModelElementInstance parentElement, string id, Type elementClass) where T : org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance
	  {
			  elementClass = typeof(T);
		T element = modelInstance.newInstance(elementClass);
		element.setAttributeValue("id", id, true);
		parentElement.addChildElement(element);
		return element;
	  }

	  protected internal virtual IList<CaseDefinitionEntity> transform()
	  {
		// convert the model to the XML string representation
		Stream outputStream = new MemoryStream();
		Cmmn.writeModelToStream(outputStream, modelInstance);
		Stream inputStream = IoUtil.convertOutputStreamToInputStream(outputStream);

		sbyte[] model = org.camunda.bpm.engine.impl.util.IoUtil.readInputStream(inputStream, "model");

		ResourceEntity resource = new ResourceEntity();
		resource.Bytes = model;
		resource.Name = "test";

		transformer.Resource = resource;
		IList<CaseDefinitionEntity> definitions = transformer.transform();

		IoUtil.closeSilently(outputStream);
		IoUtil.closeSilently(inputStream);

		return definitions;
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+                    +-----------------+
	  ///   | Case1            \                   | aCaseDefinition |
	  ///   +-------------------+---+              +-----------------+
	  ///   |                       |                      |
	  ///   |                       |   ==>        +-----------------+
	  ///   |                       |              |  aCasePlanModel |
	  ///   |                       |              +-----------------+
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCasePlanModel()
	  public virtual void testCasePlanModel()
	  {
		// given

		// when
		IList<CaseDefinitionEntity> caseDefinitions = transform();

		// then
		assertEquals(1, caseDefinitions.Count);

		CmmnCaseDefinition caseModel = caseDefinitions[0];

		IList<CmmnActivity> activities = caseModel.Activities;

		assertEquals(1, activities.Count);

		CmmnActivity casePlanModelActivity = activities[0];
		assertEquals(casePlanModel.Id, casePlanModelActivity.Id);
		assertTrue(casePlanModelActivity.Activities.Count == 0);
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+                    +-----------------+
	  ///   | Case1            \                   | aCaseDefinition |
	  ///   +-------------------+---+              +-----------------+
	  ///   |                       |                      |
	  ///   |     +-------+         |   ==>        +-----------------+
	  ///   |     |   A   |         |              |  aCasePlanModel |
	  ///   |     +-------+         |              +-----------------+
	  ///   |                       |                      |
	  ///   +-----------------------+              +-----------------+
	  ///                                          |       A         |
	  ///                                          +-----------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityTreeWithOneHumanTask()
	  public virtual void testActivityTreeWithOneHumanTask()
	  {
		// given
		HumanTask humanTask = createElement(casePlanModel, "A", typeof(HumanTask));
		PlanItem planItem = createElement(casePlanModel, "PI_A", typeof(PlanItem));

		planItem.Definition = humanTask;

		// when
		IList<CaseDefinitionEntity> caseDefinitions = transform();

		// then
		assertEquals(1, caseDefinitions.Count);

		CaseDefinitionEntity caseDefinition = caseDefinitions[0];
		IList<CmmnActivity> activities = caseDefinition.Activities;

		CmmnActivity casePlanModelActivity = activities[0];

		IList<CmmnActivity> planItemActivities = casePlanModelActivity.Activities;
		assertEquals(1, planItemActivities.Count);

		CmmnActivity child = planItemActivities[0];
		assertEquals(planItem.Id, child.Id);
		assertTrue(child.Activities.Count == 0);
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+                                       +-----------------+
	  ///   | Case1            \                                      | aCaseDefinition |
	  ///   +-------------------+-----------------+                   +-----------------+
	  ///   |                                     |                            |
	  ///   |     +------------------------+      |                   +-----------------+
	  ///   |    / X                        \     |                   |  aCasePlanModel |
	  ///   |   +    +-------+  +-------+    +    |                   +-----------------+
	  ///   |   |    |   A   |  |   B   |    |    |  ==>                       |
	  ///   |   +    +-------+  +-------+    +    |                   +-----------------+
	  ///   |    \                          /     |                   |        X        |
	  ///   |     +------------------------+      |                   +-----------------+
	  ///   |                                     |                           / \
	  ///   +-------------------------------------+                          /   \
	  ///                                                 +-----------------+     +-----------------+
	  ///                                                 |        A        |     |        B        |
	  ///                                                 +-----------------+     +-----------------+
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityTreeWithOneStageAndNestedHumanTasks()
	  public virtual void testActivityTreeWithOneStageAndNestedHumanTasks()
	  {
		// given
		Stage stage = createElement(casePlanModel, "X", typeof(Stage));
		HumanTask humanTaskA = createElement(casePlanModel, "A", typeof(HumanTask));
		HumanTask humanTaskB = createElement(casePlanModel, "B", typeof(HumanTask));

		PlanItem planItemX = createElement(casePlanModel, "PI_X", typeof(PlanItem));
		PlanItem planItemA = createElement(stage, "PI_A", typeof(PlanItem));
		PlanItem planItemB = createElement(stage, "PI_B", typeof(PlanItem));

		planItemX.Definition = stage;
		planItemA.Definition = humanTaskA;
		planItemB.Definition = humanTaskB;

		// when
		IList<CaseDefinitionEntity> caseDefinitions = transform();

		// then
		assertEquals(1, caseDefinitions.Count);

		CaseDefinitionEntity caseDefinition = caseDefinitions[0];
		IList<CmmnActivity> activities = caseDefinition.Activities;

		CmmnActivity casePlanModelActivity = activities[0];

		IList<CmmnActivity> children = casePlanModelActivity.Activities;
		assertEquals(1, children.Count);

		CmmnActivity planItemStage = children[0];
		assertEquals(planItemX.Id, planItemStage.Id);

		children = planItemStage.Activities;
		assertEquals(2, children.Count);

		CmmnActivity childPlanItem = children[0];
		assertEquals(planItemA.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

		childPlanItem = children[1];
		assertEquals(planItemB.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+                                                    +-----------------+
	  ///   | Case1            \                                                   | aCaseDefinition |
	  ///   +-------------------+-------------------+                              +-----------------+
	  ///   |                                       |                                       |
	  ///   |  +-------+                            |                              +-----------------+
	  ///   |  |  A1   |                            |              +---------------|  aCasePlanModel |---------------+
	  ///   |  +-------+                            |              |               +-----------------+               |
	  ///   |                                       |              |                        |                        |
	  ///   |    +------------------------+         |      +-----------------+     +-----------------+      +-----------------+
	  ///   |   / X1                       \        |      |       A1        |     |        X1       |      |        Y        |-----------+
	  ///   |  +    +-------+  +-------+    +       |      +-----------------+     +-----------------+      +-----------------+           |
	  ///   |  |    |  A2   |  |   B   |    |       |                                      / \                                           / \
	  ///   |  +    +-------+  +-------+    +       |                                     /   \                                         /   \
	  ///   |   \                          /        |                    +---------------+     +---------------+     +-----------------+     +-----------------+
	  ///   |    +------------------------+         |                    |      A2       |     |      B        |     |        C        |     |       X2        |
	  ///   |                                       |                    +---------------+     +---------------+     +-----------------+     +-----------------+
	  ///   |    +-----------------------------+    |  ==>                                                                                          / \
	  ///   |   / Y                             \   |                                                                              +---------------+   +---------------+
	  ///   |  +    +-------+                    +  |                                                                              |      A1       |   |       B       |
	  ///   |  |    |   C   |                    |  |                                                                              +---------------+   +---------------+
	  ///   |  |    +-------+                    |  |
	  ///   |  |                                 |  |
	  ///   |  |   +------------------------+    |  |
	  ///   |  |  / X2                       \   |  |
	  ///   |  | +    +-------+  +-------+    +  |  |
	  ///   |  | |    |  A1   |  |   B   |    |  |  |
	  ///   |  | +    +-------+  +-------+    +  |  |
	  ///   |  |  \                          /   |  |
	  ///   |  +   +------------------------+    +  |
	  ///   |   \                               /   |
	  ///   |    +-----------------------------+    |
	  ///   |                                       |
	  ///   +---------------------------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNestedStages()
	  public virtual void testNestedStages()
	  {
		// given
		Stage stageX = createElement(casePlanModel, "X", typeof(Stage));
		Stage stageY = createElement(casePlanModel, "Y", typeof(Stage));
		HumanTask humanTaskA = createElement(casePlanModel, "A", typeof(HumanTask));
		HumanTask humanTaskB = createElement(casePlanModel, "B", typeof(HumanTask));
		HumanTask humanTaskC = createElement(casePlanModel, "C", typeof(HumanTask));

		PlanItem planItemA1 = createElement(casePlanModel, "PI_A1", typeof(PlanItem));
		planItemA1.Definition = humanTaskA;

		PlanItem planItemX1 = createElement(casePlanModel, "PI_X1", typeof(PlanItem));
		planItemX1.Definition = stageX;
		PlanItem planItemA2 = createElement(stageX, "PI_A2", typeof(PlanItem));
		planItemA2.Definition = humanTaskA;
		PlanItem planItemB = createElement(stageX, "PI_B", typeof(PlanItem));
		planItemB.Definition = humanTaskB;

		PlanItem planItemY = createElement(casePlanModel, "PI_Y", typeof(PlanItem));
		planItemY.Definition = stageY;
		PlanItem planItemC = createElement(stageY, "PI_C", typeof(PlanItem));
		planItemC.Definition = humanTaskC;
		PlanItem planItemX2 = createElement(stageY, "PI_X2", typeof(PlanItem));
		planItemX2.Definition = stageX;

		// when
		IList<CaseDefinitionEntity> caseDefinitions = transform();

		// then
		assertEquals(1, caseDefinitions.Count);

		CaseDefinitionEntity caseDefinition = caseDefinitions[0];
		IList<CmmnActivity> activities = caseDefinition.Activities;

		CmmnActivity casePlanModelActivity = activities[0];

		IList<CmmnActivity> children = casePlanModelActivity.Activities;
		assertEquals(3, children.Count);

		CmmnActivity childPlanItem = children[0];
		assertEquals(planItemA1.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

		childPlanItem = children[1];
		assertEquals(planItemX1.Id, childPlanItem.Id);

		IList<CmmnActivity> childrenOfX1 = childPlanItem.Activities;
		assertFalse(childrenOfX1.Count == 0);
		assertEquals(2, childrenOfX1.Count);

		childPlanItem = childrenOfX1[0];
		assertEquals(planItemA2.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

		childPlanItem = childrenOfX1[1];
		assertEquals(planItemB.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

		childPlanItem = children[2];
		assertEquals(planItemY.Id, childPlanItem.Id);

		IList<CmmnActivity> childrenOfY = childPlanItem.Activities;
		assertFalse(childrenOfY.Count == 0);
		assertEquals(2, childrenOfY.Count);

		childPlanItem = childrenOfY[0];
		assertEquals(planItemC.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

		childPlanItem = childrenOfY[1];
		assertEquals(planItemX2.Id, childPlanItem.Id);

		IList<CmmnActivity> childrenOfX2 = childPlanItem.Activities;
		assertFalse(childrenOfX2.Count == 0);
		assertEquals(2, childrenOfX2.Count);

		childPlanItem = childrenOfX2[0];
		assertEquals(planItemA2.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

		childPlanItem = childrenOfX2[1];
		assertEquals(planItemB.Id, childPlanItem.Id);
		assertTrue(childPlanItem.Activities.Count == 0);

	  }

	}

}