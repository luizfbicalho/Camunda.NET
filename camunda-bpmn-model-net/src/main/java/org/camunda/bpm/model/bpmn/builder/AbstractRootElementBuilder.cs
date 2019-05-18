﻿using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using RootElement = org.camunda.bpm.model.bpmn.instance.RootElement;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractRootElementBuilder<B, E> : AbstractBaseElementBuilder<B, E> where B : AbstractRootElementBuilder<B, E> where E : org.camunda.bpm.model.bpmn.instance.RootElement
	{

	  protected internal AbstractRootElementBuilder(BpmnModelInstance modelInstance, E element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }
	}

}