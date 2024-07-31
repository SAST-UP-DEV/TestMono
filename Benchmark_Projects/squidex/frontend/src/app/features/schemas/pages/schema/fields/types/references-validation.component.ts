/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Component, Input } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { FieldDto, LanguageDto, ReferencesFieldPropertiesDto, SchemaTagSource } from '@app/shared';

@Component({
    selector: 'sqx-references-validation[field][fieldForm][languages][properties]',
    styleUrls: ['references-validation.component.scss'],
    templateUrl: 'references-validation.component.html',
})
export class ReferencesValidationComponent {
    @Input()
    public fieldForm!: UntypedFormGroup;

    @Input()
    public field!: FieldDto;

    @Input()
    public properties!: ReferencesFieldPropertiesDto;

    @Input()
    public languages!: ReadonlyArray<LanguageDto>;

    @Input()
    public isLocalizable?: boolean | null;

    constructor(
        public readonly schemasSource: SchemaTagSource,
    ) {
    }
}
