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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using InputParameter = org.camunda.bpm.engine.impl.core.variable.mapping.InputParameter;
	using IoMapping = org.camunda.bpm.engine.impl.core.variable.mapping.IoMapping;
	using OutputParameter = org.camunda.bpm.engine.impl.core.variable.mapping.OutputParameter;
	using ListValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ListValueProvider;
	using MapValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.MapValueProvider;
	using NullValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.NullValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ElValueProvider = org.camunda.bpm.engine.impl.el.ElValueProvider;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptValueProvider = org.camunda.bpm.engine.impl.scripting.ScriptValueProvider;
	using ScriptUtil = org.camunda.bpm.engine.impl.util.ScriptUtil;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;

	/// <summary>
	/// Helper methods to reused for common parsing tasks.
	/// </summary>
	public sealed class BpmnParseUtil
	{

	  /// <summary>
	  /// Returns the camunda extension element in the camunda namespace
	  /// and the given name.
	  /// </summary>
	  /// <param name="element"> the parent element of the extension element </param>
	  /// <param name="extensionElementName"> the name of the extension element to find </param>
	  /// <returns> the extension element or null if not found </returns>
	  public static Element findCamundaExtensionElement(Element element, string extensionElementName)
	  {
		Element extensionElements = element.element("extensionElements");
		if (extensionElements != null)
		{
		  return extensionElements.elementNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, extensionElementName);
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Returns the <seealso cref="IoMapping"/> of an element.
	  /// </summary>
	  /// <param name="element"> the element to parse </param>
	  /// <returns> the input output mapping or null if non defined </returns>
	  /// <exception cref="BpmnParseException"> if a input/output parameter element is malformed </exception>
	  public static IoMapping parseInputOutput(Element element)
	  {
		Element inputOutputElement = element.elementNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "inputOutput");
		if (inputOutputElement != null)
		{
		  IoMapping ioMapping = new IoMapping();
		  parseCamundaInputParameters(inputOutputElement, ioMapping);
		  parseCamundaOutputParameters(inputOutputElement, ioMapping);
		  return ioMapping;
		}
		return null;
	  }

	  /// <summary>
	  /// Parses all input parameters of an input output element and adds them to
	  /// the <seealso cref="IoMapping"/>.
	  /// </summary>
	  /// <param name="inputOutputElement"> the input output element to process </param>
	  /// <param name="ioMapping"> the input output mapping to add input parameters to </param>
	  /// <exception cref="BpmnParseException"> if a input parameter element is malformed </exception>
	  public static void parseCamundaInputParameters(Element inputOutputElement, IoMapping ioMapping)
	  {
		IList<Element> inputParameters = inputOutputElement.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "inputParameter");
		foreach (Element inputParameterElement in inputParameters)
		{
		  parseInputParameterElement(inputParameterElement, ioMapping);
		}
	  }

	  /// <summary>
	  /// Parses all output parameters of an input output element and adds them to
	  /// the <seealso cref="IoMapping"/>.
	  /// </summary>
	  /// <param name="inputOutputElement"> the input output element to process </param>
	  /// <param name="ioMapping"> the input output mapping to add input parameters to </param>
	  /// <exception cref="BpmnParseException"> if a output parameter element is malformed </exception>
	  public static void parseCamundaOutputParameters(Element inputOutputElement, IoMapping ioMapping)
	  {
		IList<Element> outputParameters = inputOutputElement.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "outputParameter");
		foreach (Element outputParameterElement in outputParameters)
		{
		  parseOutputParameterElement(outputParameterElement, ioMapping);
		}
	  }

	  /// <summary>
	  /// Parses a input parameter and adds it to the <seealso cref="IoMapping"/>.
	  /// </summary>
	  /// <param name="inputParameterElement"> the input parameter element </param>
	  /// <param name="ioMapping"> the mapping to add the element </param>
	  /// <exception cref="BpmnParseException"> if the input parameter element is malformed </exception>
	  public static void parseInputParameterElement(Element inputParameterElement, IoMapping ioMapping)
	  {
		string nameAttribute = inputParameterElement.attribute("name");
		if (string.ReferenceEquals(nameAttribute, null) || nameAttribute.Length == 0)
		{
		  throw new BpmnParseException("Missing attribute 'name' for inputParameter", inputParameterElement);
		}

		ParameterValueProvider valueProvider = parseNestedParamValueProvider(inputParameterElement);

		// add parameter
		ioMapping.addInputParameter(new InputParameter(nameAttribute, valueProvider));
	  }

	  /// <summary>
	  /// Parses a output parameter and adds it to the <seealso cref="IoMapping"/>.
	  /// </summary>
	  /// <param name="outputParameterElement"> the output parameter element </param>
	  /// <param name="ioMapping"> the mapping to add the element </param>
	  /// <exception cref="BpmnParseException"> if the output parameter element is malformed </exception>
	  public static void parseOutputParameterElement(Element outputParameterElement, IoMapping ioMapping)
	  {
		string nameAttribute = outputParameterElement.attribute("name");
		if (string.ReferenceEquals(nameAttribute, null) || nameAttribute.Length == 0)
		{
		  throw new BpmnParseException("Missing attribute 'name' for outputParameter", outputParameterElement);
		}

		ParameterValueProvider valueProvider = parseNestedParamValueProvider(outputParameterElement);

		// add parameter
		ioMapping.addOutputParameter(new OutputParameter(nameAttribute, valueProvider));
	  }

	  /// <exception cref="BpmnParseException"> if the parameter is invalid </exception>
	  protected internal static ParameterValueProvider parseNestedParamValueProvider(Element element)
	  {
		// parse value provider
		if (element.elements().Count == 0)
		{
		  return parseParamValueProvider(element);

		}
		else if (element.elements().Count == 1)
		{
		  return parseParamValueProvider(element.elements()[0]);

		}
		else
		{
		  throw new BpmnParseException("Nested parameter can at most have one child element", element);
		}
	  }

	  /// <exception cref="BpmnParseException"> if the parameter is invalid </exception>
	  protected internal static ParameterValueProvider parseParamValueProvider(Element parameterElement)
	  {

		// LIST
		if ("list".Equals(parameterElement.TagName))
		{
		  IList<ParameterValueProvider> providerList = new List<ParameterValueProvider>();
		  foreach (Element element in parameterElement.elements())
		  {
			// parse nested provider
			providerList.Add(parseParamValueProvider(element));
		  }
		  return new ListValueProvider(providerList);
		}

		// MAP
		if ("map".Equals(parameterElement.TagName))
		{
		  SortedDictionary<ParameterValueProvider, ParameterValueProvider> providerMap = new SortedDictionary<ParameterValueProvider, ParameterValueProvider>();
		  foreach (Element entryElement in parameterElement.elements("entry"))
		  {
			// entry must provide key
			string keyAttribute = entryElement.attribute("key");
			if (string.ReferenceEquals(keyAttribute, null) || keyAttribute.Length == 0)
			{
			  throw new BpmnParseException("Missing attribute 'key' for 'entry' element", entryElement);
			}
			// parse nested provider
			providerMap[new ElValueProvider(ExpressionManager.createExpression(keyAttribute))] = parseNestedParamValueProvider(entryElement);
		  }
		  return new MapValueProvider(providerMap);
		}

		// SCRIPT
		if ("script".Equals(parameterElement.TagName))
		{
		  ExecutableScript executableScript = parseCamundaScript(parameterElement);
		  if (executableScript != null)
		  {
			return new ScriptValueProvider(executableScript);
		  }
		  else
		  {
			return new NullValueProvider();
		  }
		}

		string textContent = parameterElement.Text.Trim();
		if (textContent.Length > 0)
		{
			// EL
			return new ElValueProvider(ExpressionManager.createExpression(textContent));
		}
		else
		{
		  // NULL value
		  return new NullValueProvider();
		}

	  }

	  /// <summary>
	  /// Parses a camunda script element.
	  /// </summary>
	  /// <param name="scriptElement"> the script element ot parse </param>
	  /// <returns> the generated executable script </returns>
	  /// <exception cref="BpmnParseException"> if the a attribute is missing or the script cannot be processed </exception>
	  public static ExecutableScript parseCamundaScript(Element scriptElement)
	  {
		string scriptLanguage = scriptElement.attribute("scriptFormat");
		if (string.ReferenceEquals(scriptLanguage, null) || scriptLanguage.Length == 0)
		{
		  throw new BpmnParseException("Missing attribute 'scriptFormatAttribute' for 'script' element", scriptElement);
		}
		else
		{
		  string scriptResource = scriptElement.attribute("resource");
		  string scriptSource = scriptElement.Text;
		  try
		  {
			return ScriptUtil.getScript(scriptLanguage, scriptSource, scriptResource, ExpressionManager);
		  }
		  catch (ProcessEngineException e)
		  {
			throw new BpmnParseException("Unable to process script", scriptElement, e);
		  }
		}
	  }

	  protected internal static ExpressionManager ExpressionManager
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ExpressionManager;
		  }
	  }

	}

}