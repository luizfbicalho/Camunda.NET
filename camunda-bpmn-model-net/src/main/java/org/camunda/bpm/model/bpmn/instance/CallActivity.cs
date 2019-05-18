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
namespace org.camunda.bpm.model.bpmn.instance
{
	using CallActivityBuilder = org.camunda.bpm.model.bpmn.builder.CallActivityBuilder;

	/// <summary>
	/// The BPMN callActivity element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public interface CallActivity : Activity
	{

	  CallActivityBuilder builder();

	  string CalledElement {get;set;}


	  /// <summary>
	  /// camunda extensions </summary>

	  /// @deprecated use isCamundaAsyncBefore() instead. 
	  [Obsolete("use isCamundaAsyncBefore() instead.")]
	  bool CamundaAsync {get;set;}


	  string CamundaCalledElementBinding {get;set;}


	  string CamundaCalledElementVersion {get;set;}


	  string CamundaCalledElementVersionTag {get;set;}


	  string CamundaCaseRef {get;set;}


	  string CamundaCaseBinding {get;set;}


	  string CamundaCaseVersion {get;set;}


	  string CamundaCalledElementTenantId {get;set;}


	  string CamundaCaseTenantId {get;set;}


	  string CamundaVariableMappingClass {get;set;}


	  string CamundaVariableMappingDelegateExpression {get;set;}


	}

}