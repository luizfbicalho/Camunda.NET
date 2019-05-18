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
	using QName = org.camunda.bpm.model.xml.impl.util.QName;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ChildElementCollection = org.camunda.bpm.model.xml.type.child.ChildElementCollection;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class QNameElementReferenceCollectionImpl<Target, Source> : ElementReferenceCollectionImpl<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  public QNameElementReferenceCollectionImpl(ChildElementCollection<Source> referenceSourceCollection) : base(referenceSourceCollection)
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