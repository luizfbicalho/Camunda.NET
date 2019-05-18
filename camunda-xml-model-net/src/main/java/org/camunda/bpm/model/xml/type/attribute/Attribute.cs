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
namespace org.camunda.bpm.model.xml.type.attribute
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Reference = org.camunda.bpm.model.xml.type.reference.Reference;

	/// <summary>
	/// @author meyerd
	/// </summary>
	/// @param <T> </param>
	public interface Attribute<T>
	{

	  /// <summary>
	  /// returns the value of the attribute.
	  /// </summary>
	  /// <returns> the value of the attribute. </returns>
	  T getValue(ModelElementInstance modelElement);

	  /// <summary>
	  /// sets the value of the attribute.
	  /// </summary>
	  /// <param name="value"> the value of the attribute. </param>
	  void setValue(ModelElementInstance modelElement, T value);

	  /// <summary>
	  /// sets the value of the attribute.
	  /// </summary>
	  /// <param name="value"> the value of the attribute. </param>
	  /// <param name="withReferenceUpdate"> true to update id references in other elements, false otherwise </param>
	  void setValue(ModelElementInstance modelElement, T value, bool withReferenceUpdate);

	  T DefaultValue {get;}

	  bool Required {get;}

	  /// <returns> the namespaceUri </returns>
	  string NamespaceUri {get;}

	  /// <returns> the attributeName </returns>
	  string AttributeName {get;}

	  bool IdAttribute {get;}

	  ModelElementType OwningElementType {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> getIncomingReferences();
	  IList<Reference<object>> IncomingReferences {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.type.reference.Reference<?>> getOutgoingReferences();
	  IList<Reference<object>> OutgoingReferences {get;}

	}

}