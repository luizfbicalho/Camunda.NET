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
	using BusinessRuleTaskBuilder = org.camunda.bpm.model.bpmn.builder.BusinessRuleTaskBuilder;

	/// <summary>
	/// The BPMN businessRuleTask element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface BusinessRuleTask : Task
	{

	  BusinessRuleTaskBuilder builder();

	  string Implementation {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  string CamundaClass {get;set;}


	  string CamundaDelegateExpression {get;set;}


	  string CamundaExpression {get;set;}


	  string CamundaResultVariable {get;set;}


	  string CamundaType {get;set;}


	  string CamundaTopic {get;set;}


	  string CamundaDecisionRef {get;set;}


	  string CamundaDecisionRefBinding {get;set;}


	  string CamundaDecisionRefVersion {get;set;}


	  string CamundaDecisionRefVersionTag {get;set;}


	  string CamundaDecisionRefTenantId {get;set;}


	  string CamundaMapDecisionResult {get;set;}


	  string CamundaTaskPriority {get;set;}


	}

}