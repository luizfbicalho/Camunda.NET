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
	using AttributeImpl = org.camunda.bpm.model.xml.impl.type.attribute.AttributeImpl;
	using QName = org.camunda.bpm.model.xml.impl.util.QName;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class QNameAttributeReferenceImpl<T> : AttributeReferenceImpl<T> where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  /// <summary>
	  /// Create a new QName reference outgoing from the reference source attribute
	  /// </summary>
	  /// <param name="referenceSourceAttribute"> the reference source attribute </param>
	  public QNameAttributeReferenceImpl(AttributeImpl<string> referenceSourceAttribute) : base(referenceSourceAttribute)
	  {
	  }

	  public override string getReferenceIdentifier(ModelElementInstance referenceSourceElement)
	  {
		string identifier = base.getReferenceIdentifier(referenceSourceElement);
		if (!string.ReferenceEquals(identifier, null))
		{
		  QName qName = QName.parseQName(identifier);
		  return qName.LocalName;
		}
		else
		{
		  return null;
		}
	  }

	}

}