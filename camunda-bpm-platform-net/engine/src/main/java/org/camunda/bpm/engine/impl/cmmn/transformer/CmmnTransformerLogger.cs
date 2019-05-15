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
namespace org.camunda.bpm.engine.impl.cmmn.transformer
{

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class CmmnTransformerLogger : ProcessEngineLogger
	{

	  public virtual ProcessEngineException transformResourceException(string name, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("001", "Could not transform resource '{}'.", name), cause);
	  }

	  public virtual ProcessEngineException parseProcessException(string name, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Error while parsing process of resource '{}'.", name), cause);
	  }

	  public virtual void ignoredSentryWithMissingCondition(string id)
	  {
		logInfo("003", "Sentry with id '{}' will be ignored. Reason: Neither ifPart nor onParts are defined with a condition.", id);
	  }

	  public virtual void ignoredSentryWithInvalidParts(string id)
	  {
		logInfo("004", "Sentry with id '{}' will be ignored. Reason: ifPart and all onParts are not valid.", id);
	  }

	  public virtual void ignoredUnsupportedAttribute(string attribute, string element, string id)
	  {
		logInfo("005", "The attribute '{}' based on the element '{}' of the sentry with id '{}' is not supported and will be ignored.", attribute, element, id);
	  }

	  public virtual void multipleIgnoredConditions(string id)
	  {
		logInfo("006", "The ifPart of the sentry with id '{}' has more than one condition. " + "Only the first one will be used and the other conditions will be ignored.", id);
	  }

	  public virtual CmmnTransformException nonMatchingVariableEvents(string id)
	  {
		return new CmmnTransformException(exceptionMessage("007", "The variableOnPart of the sentry with id '{}' must have one valid variable event. ", id));
	  }

	  public virtual CmmnTransformException emptyVariableName(string id)
	  {
		return new CmmnTransformException(exceptionMessage("008", "The variableOnPart of the sentry with id '{}' must have variable name. ", id));
	  }
	}

}