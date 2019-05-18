﻿using System;
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
namespace org.camunda.bpm.model.xml.type
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Attribute = org.camunda.bpm.model.xml.type.attribute.Attribute;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ModelElementType
	{

	  string TypeName {get;}

	  string TypeNamespace {get;}

	  Type InstanceType {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.attribute.Attribute<?>> getAttributes();
	  IList<Attribute<object>> Attributes {get;}

	  ModelElementInstance newInstance(ModelInstance modelInstance);

	  ModelElementType BaseType {get;}

	  bool Abstract {get;}

	  ICollection<ModelElementType> ExtendingTypes {get;}

	  ICollection<ModelElementType> AllExtendingTypes {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.model.xml.type.attribute.Attribute<?> getAttribute(String attributeName);
	  Attribute<object> getAttribute(string attributeName);

	  Model Model {get;}

	  ICollection<ModelElementInstance> getInstances(ModelInstance modelInstanceImpl);

	  IList<ModelElementType> ChildElementTypes {get;}

	  IList<ModelElementType> AllChildElementTypes {get;}

	}

}