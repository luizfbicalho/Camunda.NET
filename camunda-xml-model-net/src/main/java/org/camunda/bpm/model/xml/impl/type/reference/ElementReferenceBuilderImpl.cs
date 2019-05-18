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
namespace org.camunda.bpm.model.xml.impl.type.reference
{
	using ChildElementImpl = org.camunda.bpm.model.xml.impl.type.child.ChildElementImpl;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ElementReference = org.camunda.bpm.model.xml.type.reference.ElementReference;
	using ElementReferenceBuilder = org.camunda.bpm.model.xml.type.reference.ElementReferenceBuilder;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ElementReferenceBuilderImpl<Target, Source> : ElementReferenceCollectionBuilderImpl<Target, Source>, ElementReferenceBuilder<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  public ElementReferenceBuilderImpl(Type<Source> childElementType, Type<Target> referenceTargetClass, ChildElementImpl<Source> child) : base(childElementType, referenceTargetClass, child)
	  {
		this.elementReferenceCollectionImpl = new ElementReferenceImpl<Target, Source>(child);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public org.camunda.bpm.model.xml.type.reference.ElementReference<Target,Source> build()
	  public override ElementReference<Target, Source> build()
	  {
		return (ElementReference<Target, Source>) elementReferenceCollectionImpl;
	  }

	}

}